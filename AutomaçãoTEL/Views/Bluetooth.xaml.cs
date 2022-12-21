using AutomaçãoTEL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AutomaçãoTEL.Views
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class Bluetooth : Page
    {
        public StorageFolder temporaryFolder = ApplicationData.Current.TemporaryFolder;
        public BTViewModel Controler { get; set; }
        public IList<string> Cache { get; set; }
        public StorageFile CacheFileBT { get; set; }
        public StorageFile CacheFileModuBT { get; set; }
        public Bluetooth()
        {
            this.InitializeComponent();
            CreateCache();
            this.Controler = new BTViewModel();
            this.Controler.AttList();
            this.Controler.TsItemsIsOn = false;
        }

        async void CreateCache()
        {
            try
            {
                CacheFileBT = await temporaryFolder.CreateFileAsync("BTCache.txt", CreationCollisionOption.ReplaceExisting);
                CacheFileModuBT = await temporaryFolder.CreateFileAsync("ModuCacheBT.txt", CreationCollisionOption.ReplaceExisting);
            }
            catch (Exception)
            {
                CacheFileBT = await temporaryFolder.GetFileAsync("BTCache.txt");
                CacheFileModuBT = await temporaryFolder.GetFileAsync("ModuCacheBT.txt");
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
            ContentDialog NoModulationsOrItems = new ContentDialog
            {
                Title = "Aviso",
                Content = LvItens.SelectedItems.Count,
                CloseButtonText = "Ok"
            };
            _ = await NoModulationsOrItems.ShowAsync();
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
                        IList<string> aux = await FileIO.ReadLinesAsync(CacheFileModuBT);
                        string a = aux[cont];
                        item.IsChecked = a == "True";
                        cont++;
                    }
                }
                else
                {
                    foreach (Item item in this.Controler.Itens)
                    {
                        IList<string> aux = await FileIO.ReadLinesAsync(CacheFileBT);
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
                await FileIO.WriteTextAsync(CacheFileModuBT, "");
                foreach (Item item in Controler.Itens)
                {
                    await FileIO.AppendTextAsync(CacheFileModuBT, item.IsChecked.ToString() + "\n");
                }
            }
            else
            {
                await FileIO.WriteTextAsync(CacheFileBT, "");
                foreach (Item item in Controler.Itens)
                {
                    await FileIO.AppendTextAsync(CacheFileBT, item.IsChecked.ToString() + "\n");
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
                for (int i = 0; i < Controler.Itens.Count; i++)
                {
                    Controler.Itens[i].IsChecked = false;
                    
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
