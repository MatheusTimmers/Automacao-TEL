using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MainSpecAn;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.Net.Sockets;


// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AutomaçãoTEL.Views
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class Home : Page
    {
        public Home()
        {
            this.InitializeComponent();
        }

        private void BtIpConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TboxIpConfig.Text != "")
                {
                    MainAssay mainAssay = new MainAssay();
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

        private void BtSaveAlias_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
