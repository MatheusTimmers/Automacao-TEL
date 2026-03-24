using System;
using System.Net.Sockets;
using MainSpecAn.Session;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AutomaçãoTEL.Views
{
    public sealed partial class Home : Page
    {
        private TestSession _session;

        public Home()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is not TestSession session)
            {
                throw new InvalidOperationException("Home requer TestSession como parâmetro de navegação.");
            }

            _session = session;

            TboxIpConfig.Text = _session.Ip;

            if (_session.IsConnected)
            {
                TbConnect.Text       = "Conectado";
                TbConnect.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                TbConnect.Text       = "Não Conectado";
                TbConnect.Foreground = new SolidColorBrush(Colors.Red);
            }

            LFolder.Text   = _session.OutputFolder != null
                ? _session.OutputFolder.Name
                : "Selecione a Pasta";

            TboxAlias.Text = _session.Alias ?? "";

            base.OnNavigatedTo(e);
        }

        private void BtIpConnect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TboxIpConfig.Text))
                return;

            try
            {
                ProgRIpConnect.IsActive = true;
                _session.Connect(TboxIpConfig.Text);

                TbConnect.Text       = "Conectado";
                TbConnect.Foreground = new SolidColorBrush(Colors.Green);
            }
            catch (SocketException ex)
            {
                string msg = ex.NativeErrorCode == 10051
                    ? "Erro — A rede é inacessível"
                    : $"Erro — {ex.NativeErrorCode}";

                TbConnect.Text       = msg;
                TbConnect.Foreground = new SolidColorBrush(Colors.Red);
            }
            catch (Exception ex)
            {
                TbConnect.Text       = $"Erro — {ex.Message}";
                TbConnect.Foreground = new SolidColorBrush(Colors.Red);
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
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            _session.OutputFolder = await picker.PickSingleFolderAsync();
            LFolder.Text = _session.OutputFolder != null
                ? "Pasta: " + _session.OutputFolder.Name
                : "Erro, pasta inválida";
        }

        private void TboxAlias_TextChanged(object sender, TextChangedEventArgs e)
        {
            _session.Alias = TboxAlias.Text;
        }
    }
}
