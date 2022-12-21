using System;
using System.IO;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Windows.Graphics.Imaging;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Imaging;

namespace AutomaçãoTEL.UserFolder
{
    public class User
    {
        public string NameUser { get; private set; }
        public string PasswordUser { get; private set; }
        public SoftwareBitmap BitmapSource { get; private set; }


        public User(string nameUser, string passwordUser, SoftwareBitmap bitmapSource)
        {
            NameUser = nameUser;
            PasswordUser = GenerateHashMd5(passwordUser);
            BitmapSource = bitmapSource;
        }

        public User(string nameUser, string passwordUser)
        {
            NameUser = nameUser;
            PasswordUser = GenerateHashMd5(passwordUser);
        }

        public static string GenerateHashMd5(string input)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
        private async Task<byte[]> PlayWithData(SoftwareBitmap softwareBitmap)
        {
            // get encoded jpeg bytes
            var data = await EncodedBytes(softwareBitmap, BitmapEncoder.JpegEncoderId);
            return data;

        }

        private async Task<byte[]> EncodedBytes(SoftwareBitmap soft, Guid encoderId)
        {
            byte[] array = null;

            using (var ms = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, ms);
                encoder.SetSoftwareBitmap(soft);

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception) { return new byte[0]; }

                array = new byte[ms.Size];
                await ms.ReadAsync(array.AsBuffer(), (uint)ms.Size, InputStreamOptions.None);
            }
            return array;
        }

        private async Task<SoftwareBitmapSource> EncodedBitMap(MemoryStream ms)
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(ms.AsRandomAccessStream());
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

            SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap,
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied);

            SoftwareBitmapSource BitmapSource = new SoftwareBitmapSource();
            await BitmapSource.SetBitmapAsync(softwareBitmapBGR8);
            return BitmapSource;
        }


        public async void CreateUser()
        {
            DataBaseAutentificator autentificator = new DataBaseAutentificator();
            autentificator.ClearParameters();
            autentificator.AddParameters("@Name", NameUser);
            autentificator.AddParameters("@PasswordUser", PasswordUser);
            autentificator.AddParameters("@BitmapSource", await PlayWithData(BitmapSource));
            autentificator.ExecuteCommand(CommandType.Text, $"INSERT INTO UserData(NAME, USERPASSWORD, USERIMAGE)" +
                                                                 $"VALUES (@Name,@PasswordUser,@BitmapSource);");

        }

        public bool Login()
        {
            DataBaseAutentificator autentificator = new DataBaseAutentificator();
            var query = 1;
            autentificator.ExecuteCommand(CommandType.Text, $"SELECT NAME, USERPASSWORD FROM UserData" +
                                                                $" WHERE NAME = '{NameUser}' AND USERPASSWORD = '{PasswordUser}';");
            if (query == 0)
            {
                return false;
            }
            return true;     
            
        }

        public async Task<SoftwareBitmapSource> LoadImageAsync()
        {
            DataBaseAutentificator autentificator = new DataBaseAutentificator();
            autentificator.ClearParameters();
            autentificator.AddParameters("@Name", NameUser);
            autentificator.AddParameters("@PasswordUser", PasswordUser);
            var query = autentificator.ExecuteQuery(CommandType.Text, $"SELECT USERIMAGE FROM UserData" +
                                                                $" WHERE NAME = @Name AND USERPASSWORD = @PasswordUser;");

            return await EncodedBitMap(query);
        }


        public bool CompareMD5(string passWordDB, string PW_MD5)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                var PW = GenerateHashMd5(passWordDB);
                if (ChecksHash(md5Hash, PW_MD5, PW))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool ChecksHash(MD5 md5Hash, string input, string hash)
        {
            StringComparer compara = StringComparer.OrdinalIgnoreCase;

            if (0 == compara.Compare(input, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
