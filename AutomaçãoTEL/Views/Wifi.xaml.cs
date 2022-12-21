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
        public IList<string> Cache { get; set; }
        public StorageFile CacheFileWifi { get; set; }
        public StorageFile CacheFileModu { get; set; }
        public Wifi()
        {
            this.InitializeComponent();
            CreateCache();
            this.Controler = new WifiViewModel();
            this.Controler.AttList();
            this.Controler.TsItemsIsOn = false;
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


        #endregion

        private async void BtConfirme_Click(object sender, RoutedEventArgs e)
        {
            
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

        private async void TsItems_Toggled(object sender, RoutedEventArgs e)
        {
            if (TsItems.IsOn)
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


        private void Item_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).Content.ToString() == "Selecionar Todos")
            {
                foreach (Item item in Controler.Itens)
                {
                    item.IsChecked = true;
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
