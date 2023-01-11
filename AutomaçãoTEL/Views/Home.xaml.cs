using System;
using AutomaçãoTEL;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MainSpecAn;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.Net.Sockets;
using Windows.UI.Xaml.Navigation;
using System.IO;


// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AutomaçãoTEL.Views
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class Home : Page
    {
        MainAssay mainAssay {get;set;}
        public Home()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!(e.Parameter as MainAssay).Equals(null))
            {
                mainAssay = e.Parameter as MainAssay;
                if (mainAssay.IsConnect == true)
                {
                    TboxIpConfig.Text = mainAssay.Ip;
                    TbConnect.Text = "Conectado";
                    TbConnect.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    TboxIpConfig.Text = mainAssay.Ip;
                    TbConnect.Text = "Não Conectado";
                    TbConnect.Foreground = new SolidColorBrush(Colors.Red);
                }
                if (mainAssay.Folder != null)
                {
                    LFolder.Text = mainAssay.Folder.Name;
                }
                else
                {
                    LFolder.Text = "Selecione a Pasta";
                }
                if (mainAssay.Alias != null)
                {
                    TboxAlias.Text = mainAssay.Alias;
                }
                else
                {
                    TboxAlias.Text = "";
                }

            }
            else
            {
                throw new Exception();
            }
            base.OnNavigatedTo(e);
        }

        private void BtIpConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TboxIpConfig.Text != "")
                {
                    
                    ProgRIpConnect.IsActive = true;
                    var aux = mainAssay.ConnectLan(TboxIpConfig.Text);
                    if (aux == "Ok")
                    {
                        TbConnect.Text = "Conectado";
                        TbConnect.Foreground = new SolidColorBrush(Colors.Green);
                    }
                }
            }
            catch(SocketException es)
            {
                if (es.NativeErrorCode.Equals(10051))
                {
                    TbConnect.Text = $"Erro - A rede é inacessível";
                    TbConnect.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    TbConnect.Text = $"Erro - {es.NativeErrorCode}";
                    TbConnect.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
            finally
            {
                ProgRIpConnect.IsActive = false;
            }
                
  
        }

        private async void BtSaveFolder_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FolderPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;

            mainAssay.Folder = await picker.PickSingleFolderAsync();
            if (mainAssay.Folder != null)
            {
                // Application now has read/write access to the picked file
                this.LFolder.Text = "Pasta: " + mainAssay.Folder.Name;
            }
            else
            {
                this.LFolder.Text = "Erro, pasta inválida";
            }
        }

        private void TboxAlias_TextChanged(object sender, TextChangedEventArgs e)
        {
            mainAssay.Alias = TboxAlias.Text;
        }
    }
}
