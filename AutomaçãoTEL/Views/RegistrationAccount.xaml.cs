using System;
using System.IO;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using FirebirdSql.Data.FirebirdClient;
using AutomaçãoTEL.UserFolder;



// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AutomaçãoTEL.Views
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class RegistrationAccount : Page
    {

        public SoftwareBitmap softwareBitmapBGR8 { get; private set; }
        public SoftwareBitmapSource BitmapSource { get; private set; }
        private bool Newphoto = true;

        public RegistrationAccount()
        {
            this.InitializeComponent();
        }

        private async void BtPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (Newphoto)
            {
                CameraCaptureUI captureUI = new CameraCaptureUI();
                captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
                captureUI.PhotoSettings.CroppedSizeInPixels = new Size(400, 400);

                StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

                if (photo == null)
                {
                    return;
                }

                IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap,
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied);

                BitmapSource = new SoftwareBitmapSource();
                await BitmapSource.SetBitmapAsync(softwareBitmapBGR8);

                personPicture.ProfilePicture = BitmapSource;
                Newphoto = false;
                BtPhoto.Content = "Apagar Foto";
            }
            else
            {
                Newphoto = true;
                BtPhoto.Content = "Nova Foto";
                personPicture.ProfilePicture = null;
            }
            
        }

        private void BtRegister_Click(object sender, RoutedEventArgs e)
        {
            //CRIAR ALGORTIMO DE BUSCA DE USUARIO JA CADASTRADO
            if (TboxName.Text == "" || TboxPassword.Password == "")
            {
                DisplayMissNamorePassword();
                return;
            }
            User user = new User(TboxName.Text, TboxPassword.Password, softwareBitmapBGR8);
            user.CreateUser();

        }




        private async void DisplayMissNamorePassword()
        {
            ContentDialog noWifiDialog = new ContentDialog
            {
                Title = "Nome ou senha inválidos",
                Content = "Verifique o nome ou a senha e tente novamente",
                CloseButtonText = "Ok"
            };
            _ = await noWifiDialog.ShowAsync();
        }
    }
}
