using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomaçãoTEL.ViewModel;
using MainSpecAn;
using MainSpecAn.Session;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AutomaçãoTEL.Views
{
    public sealed partial class Wifi : Page
    {
        public StorageFolder temporaryFolder = ApplicationData.Current.TemporaryFolder;
        public WifiViewModel Controler { get; set; }
        public ConfigViewModel ControlerConfig { get; set; }

        public StorageFile CacheFileWifi { get; set; }
        public StorageFile CacheFileModu { get; set; }

        private TestSession _session;

        public Wifi()
        {
            this.InitializeComponent();
            CreateCache();
            this.Controler = new WifiViewModel();
            this.ControlerConfig = new ConfigViewModel();
            this.Controler.AttList();
            this.Controler.TsItemsIsOn = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is not TestSession session)
            {
                throw new InvalidOperationException("Wifi requer TestSession como parâmetro de navegação.");
            }

            _session = session;
            LIp.Text = _session.IsConnected
                ? $"Analisador de Espectro Conectado: {_session.Ip}"
                : "Analisador de Espectro Desconectado";

            base.OnNavigatedTo(e);
        }

        async void CreateCache()
        {
            try
            {
                CacheFileWifi = await temporaryFolder.CreateFileAsync("WifiCache.txt", CreationCollisionOption.ReplaceExisting);
                CacheFileModu = await temporaryFolder.CreateFileAsync("ModuCache.txt",  CreationCollisionOption.ReplaceExisting);
            }
            catch
            {
                CacheFileWifi = await temporaryFolder.GetFileAsync("WifiCache.txt");
                CacheFileModu = await temporaryFolder.GetFileAsync("ModuCache.txt");
            }
        }

        // ── BtConfirme ─────────────────────────────────────────────────────────

        private async void BtConfirme_Click(object sender, RoutedEventArgs e)
        {
            if (_session == null || !_session.IsConnected)
            {
                await ShowDialogAsync("Não conectado", "Conecte ao instrumento na tela inicial.");
                return;
            }
            if (_session.OutputFolder == null)
            {
                await ShowDialogAsync("Pasta não selecionada", "Selecione a pasta de saída na tela inicial.");
                return;
            }

            try
            {
                await SaveCache(!TsItems.IsOn);

                List<string> listAssay = await ListAssay();
                List<string> listMod   = await ListModu();

                if (listAssay.Count == 0 || listMod.Count == 0)
                {
                    await ShowDialogAsync("Aviso", "Nenhuma modulação ou ensaio selecionado.");
                    return;
                }

                DisplayInitAssay(listAssay.Count * listMod.Count);

                var runner = new AssayRunner(_session);
                await RunAssaysAsync(runner, listAssay, listMod);

                EndAssay();
            }
            catch (Exception ex)
            {
                EndAssay();
                await ShowDialogAsync("Erro durante ensaio", ex.Message);
            }
        }

        private async Task RunAssaysAsync(AssayRunner runner, List<string> listAssay, List<string> listMod)
        {
            for (int i = 0; i < listMod.Count; i++)
            {
                string modu = listMod[i];
                string bw   = GetBandwidthForModulation(modu);
                ControlerConfig.Configs.WifiConfigs.Bw = "Largura de " + bw;

                for (int j = 0; j < listAssay.Count; j++)
                {
                    string assay = listAssay[j];

                    if (ControlerConfig.Configs.IsCheckedFreqI)
                        await runner.RunWifiAssayAsync("usuario", modu,
                            ControlerConfig.Configs.WifiConfigs.FreqIWifi,
                            assay, bw,
                            ControlerConfig.Configs.RefLevel,
                            ControlerConfig.Configs.Att,
                            ControlerConfig.Configs.IsCheckedPrintI);

                    if (ControlerConfig.Configs.IsCheckedFreqC)
                        await runner.RunWifiAssayAsync("usuario", modu,
                            ControlerConfig.Configs.WifiConfigs.FreqCWifi,
                            assay, bw,
                            ControlerConfig.Configs.RefLevel,
                            ControlerConfig.Configs.Att,
                            ControlerConfig.Configs.IsCheckedPrintC);

                    if (ControlerConfig.Configs.IsCheckedFreqF)
                        await runner.RunWifiAssayAsync("usuario", modu,
                            ControlerConfig.Configs.WifiConfigs.FreqFWifi,
                            assay, bw,
                            ControlerConfig.Configs.RefLevel,
                            ControlerConfig.Configs.Att,
                            ControlerConfig.Configs.IsCheckedPrintF);

                    await UpdateLoadingAsync();
                }
            }
        }

        /// <summary>Retorna a largura de banda em MHz para a modulação selecionada.</summary>
        private static string GetBandwidthForModulation(string modu) => modu switch
        {
            "802.11a" or "802.11b" or "802.11g"
            or "802.11n (20)" or "802.11ac (20)" or "802.11ax (20)"
            or "Bluetooth Low Energy" => "20",

            "802.11n (40)" or "802.11ac (40)" or "802.11ax (40)" => "40",
            "802.11n (80)" or "802.11ac (80)" or "802.11ax (80)" => "80",
            "802.11ax (160)"                                      => "160",
            _ => "20"
        };

        // ── UI helpers ─────────────────────────────────────────────────────────

        private void DisplayInitAssay(int count)
        {
            PbLoading.Value    = 0;
            PbLoading.Maximum  = count;
            TbAssayFinish.Text = $"Ensaios concluídos: 0/{count}";
            PanelLoading.Visibility = Visibility.Visible;
        }

        private void EndAssay()
        {
            PanelLoading.Visibility = Visibility.Collapsed;
        }

        private async Task UpdateLoadingAsync()
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PbLoading.Value++;
                TbAssayFinish.Text = $"Ensaios concluídos: {(int)PbLoading.Value}/{(int)PbLoading.Maximum}";
            });
        }

        // ── Cache ──────────────────────────────────────────────────────────────

        private async void TsItems_Toggled(object sender, RoutedEventArgs e)
        {
            await SaveCache(TsItems.IsOn);
        }

        private async void LoadCache()
        {
            try
            {
                var file = TsItems.IsOn ? CacheFileWifi : CacheFileModu;
                if (file == null) return;

                int cont = 0;
                foreach (Item item in this.Controler.Itens)
                {
                    IList<string> aux = await FileIO.ReadLinesAsync(file);
                    if (cont < aux.Count)
                        item.IsChecked = aux[cont] == "True";
                    cont++;
                }
            }
            catch { /* cache inválido, ignora */ }
        }

        private async Task SaveCache(bool savingAssays)
        {
            StorageFile target = savingAssays ? CacheFileWifi : CacheFileModu;
            if (target != null)
            {
                await FileIO.WriteTextAsync(target, "");
                foreach (Item item in Controler.Itens)
                    await FileIO.AppendTextAsync(target, item.IsChecked + "\n");
            }
            this.Controler.TsItemsIsOn = TsItems.IsOn;
            this.Controler.AttList();
            LoadCache();
        }

        public async Task<List<string>> ListAssay()
        {
            var list = new List<string>();
            if (CacheFileWifi == null) return list;
            IList<string> lines = await FileIO.ReadLinesAsync(CacheFileWifi);
            for (int i = 0; i < Controler.namesAssayWifi.Count && i < lines.Count; i++)
            {
                if (lines[i] == "True" && Controler.namesAssayWifi[i] != "Selecionar Todos")
                    list.Add(Controler.namesAssayWifi[i]);
            }
            return list;
        }

        public async Task<List<string>> ListModu()
        {
            var list = new List<string>();
            if (CacheFileModu == null) return list;
            IList<string> lines = await FileIO.ReadLinesAsync(CacheFileModu);
            for (int i = 0; i < Controler.namesModulations.Count && i < lines.Count; i++)
            {
                if (lines[i] == "True" && Controler.namesModulations[i] != "Selecionar Todos")
                    list.Add(Controler.namesModulations[i]);
            }
            return list;
        }

        // ── CheckBox handlers ──────────────────────────────────────────────────

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

        // ── Diálogos ──────────────────────────────────────────────────────────

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
