using System;
using AutomaçãoTEL.UserFolder;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

namespace AutomaçãoTEL.Views
{
    public sealed partial class Login : Page
    {
        private muxc.NavigationView _navView;

        public Login()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navView = e.Parameter as muxc.NavigationView;
            base.OnNavigatedTo(e);
        }

        private void BtRegister_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegistrationAccount));
            if (_navView != null)
                _navView.IsBackEnabled = true;
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Falha ao carregar página: " + e.SourcePageType.FullName);
        }

        private async void BtLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TboxName.Text) || TboxPassword.Password == "")
            {
                await ShowDialogAsync("Nome ou senha inválidos",
                    "Verifique o nome ou a senha e tente novamente.");
                return;
            }

            try
            {
                var user = new User(TboxName.Text, TboxPassword.Password);
                if (!user.Login())
                {
                    await ShowDialogAsync("Nome ou senha inválidos",
                        "Verifique o nome ou a senha e tente novamente.");
                    return;
                }

                await ShowDialogAsync("Login Efetuado", "");
                personPicture.ProfilePicture = await user.LoadImageAsync();
            }
            catch (Exception ex)
            {
                await ShowDialogAsync("Erro", "Tente Novamente — " + ex.Message);
            }
        }

        private async System.Threading.Tasks.Task ShowDialogAsync(string title, string content)
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
