using System;
using MainSpecAn.SpecAn;
using DataBaseClass;
using System.Data;
using ConnectLan;
using System.Net.Sockets;

namespace MainSpecAn
{
    public class MainAssay
    {
        public Connect ConnectMachine { get; set; }

        public string ConnectLan(string ip)
        {
            ConnectMachine = new Connect(ip, 5025, 5000);
            return "Ok";
        }


        public string AssayWifi(string user, string alias, string valFreq, string[] espurioFreq, string brand, string ip, string nameAssay, string bandwidth, string RefLevel, string Att, bool tPrints)
        {
            try
            {
                if (brand == "Keysight")
                {
                    N9010A N9010A = new N9010A(ip, 5025, 5000);
                    if (N9010A == null)
                    {
                        return "Erro No ip vagabundo";
                    }
                    switch (nameAssay)
                    {
                        case "Largura de Faixa 6db":
                            return N9010A.AssayLarguraDeBanda(user, alias, valFreq, "6", bandwidth, RefLevel, Att, tPrints);
                        case "Largura de Faixa 26db":
                            return N9010A.AssayLarguraDeBanda(user, alias, valFreq, "26", bandwidth, RefLevel, Att, tPrints);
                        case "Potencia de Pico Maxima":
                            return N9010A.AssayPotenciaDePicoMaxima(user, alias, valFreq, bandwidth, RefLevel, Att, tPrints);
                        case "Valor médio da potência máxima de saída":
                            return N9010A.AssayValorMedioDaPotenciaMaximaDeSaida(user, alias, valFreq, bandwidth, RefLevel, Att, tPrints);
                        case "pico da densidade de potência":
                            return N9010A.AssayPicoDaDensidadeDePotencia(user, alias, valFreq, bandwidth, RefLevel, Att, tPrints);
                        case "Valor_Medio_Densidade_Espectral":
                            return N9010A.AssayValorMedioDensidadeEspectral(user, alias, valFreq, bandwidth, RefLevel, Att, tPrints);
                        case "Espurios":
                            return N9010A.AssayEspurios(user, alias, espurioFreq, RefLevel, Att);
                        case "Potencia_de_Saida":
                            return N9010A.AssayPotenciaDeSaida(user, alias, valFreq, bandwidth, RefLevel, Att, tPrints);
                        case "Densidade_Espectral_de_Potencia":
                            return N9010A.AssayDensidadeEspectralDePotencia(user, alias, valFreq, bandwidth, RefLevel, Att, tPrints);
                        default: return "Erro! Ensaio não encontrado";

                    }
                }
                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        protected void SaveDb(string alias, string valFreq, string val, string nameAssay, string model, string user)
        {
            DataBaseCommands cmd = new DataBaseCommands();
            cmd.ClearParameters();
            cmd.AddParameters("@Alias", alias);
            cmd.AddParameters("@ValFreq", valFreq);
            cmd.AddParameters("@Value", val);
            cmd.AddParameters("@Model", model);
            cmd.AddParameters("@User", user);
            cmd.ExecuteCommand(CommandType.Text, $"INSERT INTO {nameAssay}(ValFreq, Value, Model , User)" +
                                                 $"VALUES (@ValFreq, @Value, @Model, @User)" +
                                                 $"WHERE Alias = @Alias;");
        }

        protected void SaveDbImage(string alias, string nameAssay ,byte[] image, string model, string user)
        {
            DataBaseCommands cmd = new DataBaseCommands();
            cmd.ClearParameters();
            cmd.AddParameters("@Alias", alias);
            cmd.AddParameters("@AssayImage", image);
            cmd.AddParameters("@Model", model);
            cmd.AddParameters("@User", user);
            cmd.ExecuteCommand(CommandType.Text, $"INSERT INTO {nameAssay}(AssayImage, Model, User)" +
                                                 $"VALUES (@AssayImage, @Model, @User)" +
                                                 $"WHERE Alias = @Alias;");
        }

        protected void SaveDbImageEspurios(string alias, string nameAssay, string printNumber, byte[] image, string model, string user)
        {
            DataBaseCommands cmd = new DataBaseCommands();
            cmd.ClearParameters();
            cmd.AddParameters("@Alias", alias);
            cmd.AddParameters("@AssayImage", image);
            cmd.AddParameters("@Model", model);
            cmd.AddParameters("@User", user);
            cmd.AddParameters("@PrintNumber", printNumber);
            cmd.ExecuteCommand(CommandType.Text, $"INSERT INTO {nameAssay}(AssayImage, Model, User)" +
                                                 $"VALUES (@AssayImage, @Model, @User)" +
                                                 $"WHERE Alias = @Alias; and @PrintNumber");
        }

    }
}
