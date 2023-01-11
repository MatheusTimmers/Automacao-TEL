using System;
using MainSpecAn.SpecAn;
using DataBaseClass;
using System.Data;
using ConnectLan;
using System.Net.Sockets;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.System;
using System.IO;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using System.Text;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Controls;
using System.Net.NetworkInformation;

namespace MainSpecAn
{
    public class MainAssay
    {
        public string Alias { get; set; }
        public StorageFolder Folder { get; set; }
        public Connect ConnectMachine { get; set; }
        public string Ip { get; set; }
        public bool? IsConnect { get; set; }


        public string ConnectLan(string ip)
        {
            this.Ip = ip;
            ConnectMachine = new Connect(ip, 5025, 5000);
            this.IsConnect = true;
            return "Ok";
        }

        public async Task AssayWifi(string user, string modu, string valFreq, string brand, string nameAssay, string bandwidth, string RefLevel, string Att, bool tPrints)
        {
            if (brand == "Keysight")
            {
                N9010A N9010A = new N9010A(Ip, 5025, 5000);
                if (N9010A == null)
                {
                    throw new Exception("Erro No ip vagabundo");
                }
                switch (nameAssay)
                {
                    case "Largura de Faixa a 6 dB":
                        await N9010A.AssayLarguraDeBanda("WIFI", user, modu, valFreq, "6", bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Largura de Faixa a 26 dB":
                        await N9010A.AssayLarguraDeBanda("WIFI", user, modu, valFreq, "26", bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Potência de Pico Máxima":
                        await N9010A.AssayPotenciaDePicoMaxima("WIFI", user, modu, valFreq, bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Valor Médio da Potência máxima de Saida":
                        await N9010A.AssayValorMedioDaPotenciaMaximaDeSaida("WIFI", user, modu, valFreq, bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Pico da Densidade de Potência":
                        await N9010A.AssayPicoDaDensidadeDePotencia("WIFI", user, modu, valFreq, bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Valor Médio da Densidade Espectral de Potência":
                        await N9010A.AssayValorMedioDensidadeEspectral("WIFI", user, modu, valFreq, bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Emissão fora de faixa":
                        // return N9010A.AssayEspurios(user, modu, espurioFreq, RefLevel, Att);
                        return;
                    case "Potencia_de_Saida":
                        await N9010A.AssayPotenciaDeSaida("WIFI", user, modu, valFreq, bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Densidade_Espectral_de_Potencia":
                        await N9010A.AssayDensidadeEspectralDePotencia("WIFI", user, modu, valFreq, bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    default: return;

                }
            }
        }

        public async Task AssayBT(string user, string modu, string valFreq, string brand, string nameAssay, string bandwidth, string RefLevel, string Att, bool tPrints)
        {
            if (brand == "Keysight")
            {
                N9010A N9010A = new N9010A(Ip, 5025, 5000);
                if (N9010A == null)
                {
                    throw new Exception("Ip Inválido");
                }
                switch (nameAssay)
                {
                    case "Largura de Faixa a 20 db": 
                        await N9010A.AssayLarguraDeBanda("BT", user, modu, valFreq, "20", bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Potência de Pico Máxima":
                        await N9010A.AssayPotenciaDePicoMaxima("BT", user, modu, valFreq, bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Separação de Canais de Salto":
                        await N9010A.AssayLarguraDeBanda("BT", user, modu, valFreq, "26", bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Numero de Ocupações":
                        await N9010A.AssayValorMedioDaPotenciaMaximaDeSaida("BT", user, modu, valFreq, bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Pico da Densidade de Potência":
                        await N9010A.AssayPicoDaDensidadeDePotencia("BT", user, modu, valFreq, bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Tempo de Ocupação":
                        await N9010A.AssayValorMedioDensidadeEspectral("BT", user, modu, valFreq, bandwidth, RefLevel, Att, tPrints, Alias, Folder);
                        return;
                    case "Emissão Fora da Faixa":
                        // return N9010A.AssayEspurios(user, modu, espurioFreq, RefLevel, Att);
                        return;
                    default: return;

                }
            }
        }





        public async Task SaveValueCSV(StorageFolder csvFolder, string nameAssay, string valFreq, string val)
        {
            StorageFile csvFile = await csvFolder.CreateFileAsync(nameAssay + ".csv", CreationCollisionOption.OpenIfExists);
            await FileIO.AppendTextAsync(csvFile, $"Freq:;{valFreq};Valor:;{val} \n");

        }



        protected async Task<StorageFolder> CreateCsv(string tech, string modu, string nameAssay, string model, string user, string alias, StorageFolder folder)
        {
            StorageFolder _ = await folder.CreateFolderAsync(alias, CreationCollisionOption.OpenIfExists);
            if (tech == "WIFI")
            {
                _ = await _.CreateFolderAsync("Wifi", CreationCollisionOption.OpenIfExists);
                _ = await _.CreateFolderAsync(modu, CreationCollisionOption.OpenIfExists);
                if (_.GetFileAsync(nameAssay) != null)
                {
                    StorageFile csvFile = await _.CreateFileAsync(nameAssay + ".csv", CreationCollisionOption.OpenIfExists);
                    await FileIO.WriteTextAsync(csvFile, $"User:;{user};Analisador:;{model}\n");
                }
            }
            return _;
        }



        protected void SaveDb(string modu, string valFreq, string val, string nameAssay, string model, string user)
        {
            DataBaseCommands cmd = new DataBaseCommands();
            cmd.ClearParameters();
            cmd.AddParameters("@modu", modu);
            cmd.AddParameters("@ValFreq", valFreq);
            cmd.AddParameters("@Value", val);
            cmd.AddParameters("@Model", model);
            cmd.AddParameters("@User", user);
            cmd.ExecuteCommand(CommandType.Text, $"INSERT INTO {nameAssay}(ValFreq, Value, Model , User)" +
                                                 $"VALUES (@ValFreq, @Value, @Model, @User)" +
                                                 $"WHERE modu = @modu;");
        }

        protected async Task SaveFolderImage(string assay ,string valFreq, byte[] image, StorageFolder folder)
        {
            StorageFile print = await folder.CreateFileAsync(valFreq + " " + assay + ".png", CreationCollisionOption.GenerateUniqueName);

            await FileIO.WriteBytesAsync(print, image);
        }

        protected void SaveDbImage(string modu, string nameAssay ,byte[] image, string model, string user)
        {
            DataBaseCommands cmd = new DataBaseCommands();
            cmd.ClearParameters();
            cmd.AddParameters("@modu", modu);
            cmd.AddParameters("@AssayImage", image);
            cmd.AddParameters("@Model", model);
            cmd.AddParameters("@User", user);
            cmd.ExecuteCommand(CommandType.Text, $"INSERT INTO {nameAssay}(AssayImage, Model, User)" +
                                                 $"VALUES (@AssayImage, @Model, @User)" +
                                                 $"WHERE modu = @modu;");
        }

        protected void SaveDbImageEspurios(string modu, string nameAssay, string printNumber, byte[] image, string model, string user)
        {
            DataBaseCommands cmd = new DataBaseCommands();
            cmd.ClearParameters();
            cmd.AddParameters("@modu", modu);
            cmd.AddParameters("@AssayImage", image);
            cmd.AddParameters("@Model", model);
            cmd.AddParameters("@User", user);
            cmd.AddParameters("@PrintNumber", printNumber);
            cmd.ExecuteCommand(CommandType.Text, $"INSERT INTO {nameAssay}(AssayImage, Model, User)" +
                                                 $"VALUES (@AssayImage, @Model, @User)" +
                                                 $"WHERE modu = @modu; and @PrintNumber");
        }


        protected async Task<ContentDialogResult> WaitUser()
        {
            ContentDialog AvisoAction = new ContentDialog
            {
                Title = "Aviso",
                Content = "Tudo certo ai, Chefia?",
                PrimaryButtonText = "Sim",
                CloseButtonText = "Não sei"
            };
            ContentDialogResult result = await AvisoAction.ShowAsync();

            return result;
        }




    }
}
