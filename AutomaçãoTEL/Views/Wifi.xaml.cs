using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using AutomaçãoTEL.ViewModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using MainSpecAn;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using System.Drawing;
using Windows.UI.Core;
using Windows.Foundation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AutomaçãoTEL.Views
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class Wifi : Page
    {
        public StorageFolder temporaryFolder = ApplicationData.Current.TemporaryFolder;
        public WifiViewModel Controler { get; set; }
        public ConfigViewModel ControlerConfig { get; set; }
        public IList<string> Cache { get; set; }
        public StorageFile CacheFileWifi { get; set; }
        public StorageFile CacheFileModu { get; set; }
        MainAssay mainAssay { get; set; }
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
            if (!(e.Parameter as MainAssay).Equals(null))
            {
                mainAssay = e.Parameter as MainAssay;
                if (mainAssay.IsConnect == true)
                {
                    LIp.Text = "Analisador de Espectro Conectado";
                }
                else
                {
                    LIp.Text = "Analisador de Espectro Desconectado";
                }
            }
            else
            {
                throw new Exception();
            }
            base.OnNavigatedTo(e);
        }


        async void CreateCache()
        {
            try
            {
                CacheFileWifi = await temporaryFolder.CreateFileAsync("WifiCache.txt", CreationCollisionOption.ReplaceExisting);
                CacheFileModu = await temporaryFolder.CreateFileAsync("ModuCache.txt", CreationCollisionOption.ReplaceExisting);
            }
            catch (Exception)
            {
                CacheFileWifi = await temporaryFolder.GetFileAsync("WifiCache.txt");
                CacheFileModu = await temporaryFolder.GetFileAsync("ModuCache.txt");
            }
        }


        #region DisplayErros

        private async Task DisplayNoModulationsOrItems()
        {
            ContentDialog NoModulationsOrItems = new ContentDialog
            {
                Title = "Aviso",
                Content = "Nenhuma modulação ou item foi selecionado",
                CloseButtonText = "Ok"
            };
            _ = await NoModulationsOrItems.ShowAsync();
        }

        private async Task DisplayWaitUser(string name, string action)
        {
            ContentDialog WaitUser = new ContentDialog
            {
                Title = "Aviso",
                Content = $"Iniciando ensaio de {name}, {action}",
                CloseButtonText = "Ok"
            };
            _ = await WaitUser.ShowAsync();
        }

        private void DisplayInitAssay(int count)
        {

            PbLoading.Value = 0;
            TbAssayFinish.Text = $"Ensaios concluidos: {PbLoading.Value}/{count}";
            PanelLoading.Visibility = Visibility.Visible;
            PbLoading.Maximum = count;
        }


        private void EndAssay()
        {
            PanelLoading.Visibility = Visibility.Collapsed;
        }


        private async Task AttLoading()
        {
            await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PbLoading.Value++;
                TbAssayFinish.Text = $"Ensaios concluidos: {PbLoading.Value}/{PbLoading.Maximum}";
            });
        }


        #endregion

        private async void BtConfirme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SaveCache(!TsItems.IsOn);
                DisplayInitAssay((await ListAssay()).Count * (await ListModu()).Count);
                await InitAssay();
                EndAssay();
            }
            catch(Exception)
            {
            }
        }

        private async Task InitAssay()
        {
            List<string> listAssay = await ListAssay();
            List<string> listMod = await ListModu();
            for (int i = 0; i < listMod.Count; i++)
            {
                for (int j = 0; j < listAssay.Count; j++)
                {
                    if (listAssay[j] == "Emissão fora de faixa")
                    {
                        if (ControlerConfig.Configs.IsCheckedFreqI)
                        {
                            await DisplayWaitUser("Emissão fora de faixa", "Selecione a frequência inicial");
                        }
                        if (ControlerConfig.Configs.IsCheckedFreqF)
                        {
                            await DisplayWaitUser("Emissão fora de faixa", "Selecione a frequência final");
                        }
                    }
                    if (listMod[i] == "802.11a" || listMod[i] == "802.11b" || listMod[i] == "802.11g" || listMod[i] == "802.11n (20)" || listMod[i] == "802.11ac (20)" || listMod[i] == "802.11ax (20)")
                    {
                        ControlerConfig.Configs.WifiConfigs.Bw = "Largura de 20";
                        if (ControlerConfig.Configs.IsCheckedFreqI)
                        {
                            await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqIWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintI);
                        }
                        if (ControlerConfig.Configs.IsCheckedFreqC)
                        {
                            await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqCWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintC);
                        }
                        if (ControlerConfig.Configs.IsCheckedFreqF)
                        {
                            await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqFWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintF);
                        }
                    }
                    else
                    {
                        if (listMod[i] == "802.11n (40)" || listMod[i] == "802.11ac (40)" || listMod[i] == "802.11ax (40)")
                        {
                            ControlerConfig.Configs.WifiConfigs.Bw = "Largura de 40";
                            if (ControlerConfig.Configs.IsCheckedFreqI)
                            {
                                await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqIWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintI);
                            }
                            if (ControlerConfig.Configs.IsCheckedFreqC)
                            {
                                await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqCWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintC);
                            }
                            if (ControlerConfig.Configs.IsCheckedFreqF)
                            {
                                await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqFWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintF);
                            }
                        }
                        else
                        {
                            ControlerConfig.Configs.WifiConfigs.Bw = "Largura de 80";
                            if (listMod[i] == "802.11n (80)" || listMod[i] == "802.11ac (80)" || listMod[i] == "802.11ax (80)")
                            {
                                if (ControlerConfig.Configs.IsCheckedFreqI)
                                {
                                    await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqIWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintI);
                                }
                                if (ControlerConfig.Configs.IsCheckedFreqC)
                                {
                                    await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqCWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintC);
                                }
                                if (ControlerConfig.Configs.IsCheckedFreqF)
                                {
                                    await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqFWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintF);
                                }
                            }
                            else
                            {
                                if (listMod[i] == "802.11ax (160)")
                                {
                                    ControlerConfig.Configs.WifiConfigs.Bw = "Largura de 160";
                                    if (ControlerConfig.Configs.IsCheckedFreqI)
                                    {
                                        await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqIWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintI);
                                    }
                                    if (ControlerConfig.Configs.IsCheckedFreqC)
                                    {
                                        await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqCWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintC);
                                    }
                                    if (ControlerConfig.Configs.IsCheckedFreqF)
                                    {
                                        await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqFWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintF);
                                    }
                                }
                                else
                                {
                                    if (listMod[i] == "Bluetooth Low Energy")
                                    {
                                        if (ControlerConfig.Configs.IsCheckedFreqI)
                                        {
                                            await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqIWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintI);
                                        }
                                        if (ControlerConfig.Configs.IsCheckedFreqC)
                                        {
                                            await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqCWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintC);
                                        }
                                        if (ControlerConfig.Configs.IsCheckedFreqF)
                                        {
                                            await mainAssay.AssayWifi("Matheus", listMod[i], ControlerConfig.Configs.WifiConfigs.FreqFWifi, "Keysight", listAssay[j], ControlerConfig.Configs.WifiConfigs.Bw, ControlerConfig.Configs.RefLevel, ControlerConfig.Configs.Att, ControlerConfig.Configs.IsCheckedPrintF);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    await AttLoading();
                }
            }
        }

        private async void LoadCache()
        {
            try
            {
                int cont = 0;
                if (!TsItems.IsOn)
                {
                    foreach (Item item in this.Controler.Itens)
                    {
                        IList<string> aux = await FileIO.ReadLinesAsync(CacheFileModu);
                        string a = aux[cont];
                        item.IsChecked = a == "True";
                        cont++;
                    }
                }
                else
                {
                    foreach (Item item in this.Controler.Itens)
                    {
                        IList<string> aux = await FileIO.ReadLinesAsync(CacheFileWifi);
                        string a = aux[cont];
                        item.IsChecked = a == "True";
                        cont++;
                    }
                }
            }
            catch (Exception)
            {

            }

        }


        private async Task SaveCache(bool state)
        {
            if (state)
            {
                await FileIO.WriteTextAsync(CacheFileModu, "");
                foreach (Item item in Controler.Itens)
                {
                    await FileIO.AppendTextAsync(CacheFileModu, item.IsChecked.ToString() + "\n");
                }
            }
            else
            {
                await FileIO.WriteTextAsync(CacheFileWifi, "");
                foreach (Item item in Controler.Itens)
                {
                    await FileIO.AppendTextAsync(CacheFileWifi, item.IsChecked.ToString() + "\n");
                }
            }
            this.Controler.TsItemsIsOn = TsItems.IsOn;
            this.Controler.AttList();
            LoadCache();
        }

        private async void TsItems_Toggled(object sender, RoutedEventArgs e)
        {
            await SaveCache(TsItems.IsOn);
        }

        public async Task<List<string>> ListAssay()
        {
            int cont = 0;
            List<string> list = new List<string>();
            for (int i = 0; i < Controler.namesAssayWifi.Count; i++)
            {
                IList<string> aux = await FileIO.ReadLinesAsync(CacheFileWifi);
                if(aux[cont] == "True")
                {
                    list.Add(Controler.namesAssayWifi[i]);
                }
                cont++;
            }
            return list;
        }

        public async Task<List<string>> ListModu()
        {
            int cont = 0;
            List<string> list = new List<string>();
            for (int i = 0; i < Controler.namesModulations.Count; i++)
            {
                IList<string> aux = await FileIO.ReadLinesAsync(CacheFileModu);
                string a = aux[cont];
                if (a == "True")
                {
                    list.Add(Controler.namesModulations[i]);
                }
                cont++;
            }
            return list;
        }


        private void Item_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Content.ToString() == "Selecionar Todos")
            {
                foreach (Item item in Controler.Itens)
                {
                    if(item.IsChecked)
                    {

                    }
                }
            }
            /*
            else
            {
                if (Controler.Itens[0].IsChecked == true)
                {
                    Controler.Itens[0].IsChecked = false;
                }
            }
            */
        }

        private void Item_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Content.ToString() == "Selecionar Todos")
            {
                foreach (Item item in Controler.Itens)
                {
                    item.IsChecked = false;
                }
            }
            /*
            else
            {
                if (Controler.Itens[0].IsChecked == true)
                {
                    Controler.Itens[0].IsChecked = false;
                }
            }
            */
        }
    }
    

}
