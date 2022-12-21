using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AutomaçãoTEL.UserFolder;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

namespace AutomaçãoTEL.Views
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class Login : Page
    {
        public muxc.NavigationView NavView { get; private set; }
        public Login()
        {
            this.InitializeComponent();
        }

        private void BtRegister_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegistrationAccount));
            NavView.IsBackEnabled = true;
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private async void BtLogin_Click(object sender, RoutedEventArgs e)
        {
            if (TboxName.Text == "" || TboxPassword.Password == "")
            {
                DisplayInvalidNameOrPassword();
                return;
            }
            try
            {
                User user = new User(TboxName.Text, TboxPassword.Password);
                if (!user.Login())
                {
                    DisplayInvalidNameOrPassword();
                    return;
                }
                LoginPerformed();


                SoftwareBitmapSource BitmapSource = new SoftwareBitmapSource();
                //await BitmapSource.SetBitmapAsync(await user.LoadImageAsync());
                personPicture.ProfilePicture = await user.LoadImageAsync();
            }
            catch (Exception a)
            {
                DisplayErro(a);
            }  
        }

        #region DisplayErros
        private async void DisplayInvalidNameOrPassword()
        {
            ContentDialog noWifiDialog = new ContentDialog
            {
                Title = "Nome ou senha inválidos",
                Content = "Verifique o nome ou a senha e tente novamente",
                CloseButtonText = "Ok"
            };
            _ = await noWifiDialog.ShowAsync();
        }


        private async void DisplayErro(Exception a)
        {
            ContentDialog Erro = new ContentDialog
            {
                Title = "Deu ERRO",
                Content = "Tente Novamente - Mensagem de erro:" + a.Message.ToString(),
                CloseButtonText = "Ok"
            };
            _ = await Erro.ShowAsync();
        }


        private async void LoginPerformed()
        {
            ContentDialog noWifiDialog = new ContentDialog
            {
                Title = "Login Efetuado",
                CloseButtonText = "Ok"
            };
            _ = await noWifiDialog.ShowAsync();
        }
        #endregion


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is muxc.NavigationView)
            {
                NavView = e.Parameter as muxc.NavigationView;
            }
            else
            {
                return;
            }
            base.OnNavigatedTo(e);
        }

    }
}
