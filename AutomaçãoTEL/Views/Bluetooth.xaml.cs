using AutomaçãoTEL.ViewModel;
using MainSpecAn;
using MainSpecAn.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AutomaçãoTEL.Views
{
    public sealed partial class Bluetooth : Page
    {
        public StorageFolder TemporaryFolder = ApplicationData.Current.TemporaryFolder;
        public BTViewModel Controler { get; set; }

        private StorageFile _cacheFileBT;
        private StorageFile _cacheFileModuBT;
        private TestSession _session;

        public Bluetooth()
        {
            this.InitializeComponent();
            this.Controler = new BTViewModel();
            this.Controler.AttList();
            this.Controler.TsItemsIsOn = false;
            CreateCache();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is TestSession session)
                _session = session;
            base.OnNavigatedTo(e);
        }

        async void CreateCache()
        {
            try
            {
                _cacheFileBT     = await TemporaryFolder.CreateFileAsync("BTCache.txt",     CreationCollisionOption.ReplaceExisting);
                _cacheFileModuBT = await TemporaryFolder.CreateFileAsync("ModuCacheBT.txt", CreationCollisionOption.ReplaceExisting);
            }
            catch
            {
                _cacheFileBT     = await TemporaryFolder.GetFileAsync("BTCache.txt");
                _cacheFileModuBT = await TemporaryFolder.GetFileAsync("ModuCacheBT.txt");
            }
        }

        private async void TsItems_Toggled(object sender, RoutedEventArgs e)
        {
            var targetFile = TsItems.IsOn ? _cacheFileModuBT : _cacheFileBT;
            if (targetFile != null)
            {
                await FileIO.WriteTextAsync(targetFile, "");
                foreach (Item item in Controler.Itens)
                    await FileIO.AppendTextAsync(targetFile, item.IsChecked + "\n");
            }

            this.Controler.TsItemsIsOn = TsItems.IsOn;
            this.Controler.AttList();
            LoadCache();
        }

        private async void LoadCache()
        {
            try
            {
                var file = TsItems.IsOn ? _cacheFileBT : _cacheFileModuBT;
                if (file == null) return;

                var lines = await FileIO.ReadLinesAsync(file);
                for (int i = 0; i < Controler.Itens.Count && i < lines.Count; i++)
                    Controler.Itens[i].IsChecked = lines[i] == "True";
            }
            catch { /* cache inválido, ignora */ }
        }

        private void Item_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox)?.Content?.ToString() == "Selecionar Todos")
            {
                foreach (Item item in Controler.Itens)
                    item.IsChecked = true;
            }
        }

        private void Item_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox)?.Content?.ToString() == "Selecionar Todos")
            {
                foreach (Item item in Controler.Itens)
                    item.IsChecked = false;
            }
        }

        private async void BtConfirme_Click(object sender, RoutedEventArgs e)
        {
            if (_session == null || !_session.IsConnected)
            {
                await ShowDialogAsync("Não conectado", "Conecte ao instrumento na tela inicial antes de iniciar os ensaios.");
                return;
            }
            if (_session.OutputFolder == null)
            {
                await ShowDialogAsync("Pasta não selecionada", "Selecione a pasta de saída na tela inicial.");
                return;
            }

            var selectedItems = Controler.Itens
                .Where(i => i.IsChecked && i.Name != "Selecionar Todos")
                .Select(i => i.Name)
                .ToList();

            if (selectedItems.Count == 0)
            {
                await ShowDialogAsync("Nenhum item selecionado", "Selecione ao menos uma modulação ou ensaio.");
                return;
            }

            var config = new ConfigViewModel();
            string refLevel = config.Configs.RefLevel ?? "0";
            string att      = config.Configs.Att      ?? "0";
            string bw       = "1"; // BT usa largura de 1 MHz por padrão

            var runner   = new AssayRunner(_session);
            var progress = new Progress<string>(_ => { });

            try
            {
                foreach (string item in selectedItems)
                {
                    if (TsItems.IsOn) // selecionou ensaios
                    {
                        string freq = config.Configs.BTConfigs.FreqCBT ?? "2441";
                        await runner.RunBTAssayAsync("usuario", "GFSK", freq, item, bw, refLevel, att, true, progress);
                    }
                }

                await ShowDialogAsync("Ensaios Concluídos", $"{selectedItems.Count} ensaio(s) executado(s) com sucesso.");
            }
            catch (Exception ex)
            {
                await ShowDialogAsync("Erro durante ensaio", ex.Message);
            }
        }

        private async Task ShowDialogAsync(string title, string content)
        {
            var dialog = new ContentDialog
            {
                Title           = title,
                Content         = content,
                CloseButtonText = "Ok"
            };
            await dialog.ShowAsync();
        }
    }
}
