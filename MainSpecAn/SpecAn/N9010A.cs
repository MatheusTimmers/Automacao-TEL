using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConnectLan;
using System.Drawing;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.System;
using Windows.Media.Audio;
using System.Numerics;

namespace MainSpecAn.SpecAn
{
    internal class N9010A : MainAssay
    {
        Connect Instr { get; set; }

        public N9010A(string instrIPAddress, int instrPortNo, int timeOut)
        {
            try
            {
                Instr = new Connect(instrIPAddress, instrPortNo, timeOut);
            }
            catch
            {
                Instr = null;
                return;
            }

        }

        #region Ensaios
        internal async Task AssayLarguraDeBanda(string tech, string user, string modu, string valFreq, string ndbDown, string bandwidth, string RefLevel, string Att, bool tPrints, string alias, StorageFolder folder)
        {
            Start();
            var t = CreateCsv(tech, modu, $"Largura de Banda a {ndbDown}", "N9010A", user, alias, folder);
            double Span = int.Parse(bandwidth) * 1.5;
            Bandwidth(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "100", "300", "ON", "MAXH", "POS", "OBW", "99", "-" + ndbDown);
            Instr.WriteLine("INIT");
            Thread.Sleep(15000);
            Instr.WriteLine("INIT:CONT OFF");
            Instr.WriteLine("FETC:OBW:XDB?");
            double aux = Convert.ToDouble(Instr.ReadLine().Replace(".", ","));
            aux /= 1000;
            string val = Convert.ToString(aux);
            var folderFile = await t;
            await SaveValueCSV(folderFile, $"Largura de Banda a {ndbDown}", valFreq, val);
            if (tPrints)
            {
                await SaveFolderImage($"Largura de Banda a {ndbDown}", valFreq, GetPrint(), folderFile);
            }
        }

        internal async Task AssayPicoDaDensidadeDePotencia(string tech, string modu, string user, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints, string alias, StorageFolder folder)
        {
            Start();
            var t = CreateCsv(tech, modu, $"PicoDaDensidadeDePotencia", "N9010A", user, alias, folder);
            double Span = int.Parse(largura_Banda) * 1.5;
            ConfiguraInstr(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "3", "10", "ON", "MAXH", "POS", "SAN");
            Instr.WriteLine("INIT");
            Thread.Sleep(15000);
            List<string> val = GetMarker();
            var folderFile = await t;
            await SaveValueCSV(folderFile, $"PicoDaDensidadeDePotencia", valFreq, val[0]);
            if (tPrints)
            {
                await SaveFolderImage($"PicoDaDensidadeDePotencia",valFreq,  GetPrint(), folderFile);
            }
        }

        internal async Task AssayValorMedioDensidadeEspectral(string tech, string user, string modu, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints, string alias, StorageFolder folder)
        {
            Start();
            var t = CreateCsv(tech, modu, $"ValorMedioDensidadeEspectral", "N9010A", user, alias, folder);
            double Span = int.Parse(largura_Banda) * 1.5;
            ConfiguraInstr(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "3", "10", "ON", "AVER", "RMS", "SAN");
            Instr.WriteLine("INIT");
            Instr.WriteLine("SWE:TIME?");
            double tempo = Convert.ToDouble(Instr.ReadLine().Replace(".", ","));
            Thread.Sleep(10000);
            Thread.Sleep((int)tempo * 1000);
            List<string> val = GetMarker();
            var folderFile = await t;
            await SaveValueCSV(folderFile, $"ValorMedioDensidadeEspectral", valFreq, val[0]);
            if (tPrints)
            {
                await SaveFolderImage($"ValorMedioDensidadeEspectral", valFreq, GetPrint(), folderFile);
            }
        }

        internal async Task AssayPotenciaDePicoMaxima(string tech, string user, string modu, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints, string alias, StorageFolder folder)
        {
            Start();
            var t = CreateCsv(tech, modu, $"PotenciaDePicoMaxima", "N9010A", user, alias, folder);
            double Span = int.Parse(largura_Banda) * 1.5;
            ChannelPower(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "1000", "3000", "ON", "MAXH", "POS", "CHP", largura_Banda);
            Instr.WriteLine("INIT");
            Thread.Sleep(15000);
            Instr.WriteLine("INIT:CONT OFF");
            Instr.WriteLine("FETC:CHP:CHP?");
            string val = Instr.ReadLine();
            var folderFile = await t;
            await SaveValueCSV(folderFile, $"PotenciaDePicoMaxima", valFreq, val);
            if (tPrints)
            {
                await SaveFolderImage($"PotenciaDePicoMaxima", valFreq, GetPrint(), folderFile);
            }

        }

        internal async Task AssayValorMedioDaPotenciaMaximaDeSaida(string tech, string user, string modu, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints, string alias, StorageFolder folder)
        {
            Start();
            var t = CreateCsv(tech, modu, $"ValorMedioDaPotenciaMaximaDeSaida", "N9010A", user, alias, folder);
            double Span = int.Parse(largura_Banda) * 1.5;
            ChannelPower(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "1000", "3000", "ON", "MAXH", "RMS", "CHP", largura_Banda);
            Instr.WriteLine("INIT");
            Thread.Sleep(15000);
            Instr.WriteLine("INIT:CONT OFF");
            Instr.WriteLine("FETC:CHP:CHP?");
            double aux = Convert.ToDouble(Instr.ReadLine().Replace(".", ","));
            aux /= 1000;
            string val = Convert.ToString(aux);
            var folderFile = await t;
            await SaveValueCSV(folderFile, $"ValorMedioDaPotenciaMaximaDeSaida", valFreq, val);
            if (tPrints)
            {
                await SaveFolderImage($"ValorMedioDaPotenciaMaximaDeSaida", valFreq, GetPrint(), folderFile);
            }
        }

        public async Task AssayPotenciaDeSaida(string tech, string user, string modu, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints, string alias, StorageFolder folder)
        {
            Start();
            double Span = int.Parse(largura_Banda) * 1.5;
            var t = CreateCsv(tech, modu, $"PotenciaDeSaida", "N9010A", user, alias, folder);
            Bandwidth(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "100", "300", "ON", "MAXH", "POS", "OBW", "99", "-26");
            Instr.WriteLine("INIT");
            Thread.Sleep(15000);
            Instr.WriteLine("INIT:CONT OFF");
            Instr.WriteLine("FETC:OBW:XDB?");
            double aux = Convert.ToDouble(Instr.ReadLine().Replace(".", ","));
            aux /= 1000000;
            string aux1 = Convert.ToString(aux);
            ChannelPower(valFreq, "DBM", Att, RefLevel, Span.ToString(), "1000", "3000", "ON", "AVER", "RMS", "CHP", aux1);
            Instr.WriteLine("INIT");
            Thread.Sleep(15000);
            Instr.WriteLine("INIT:CONT OFF");
            Instr.WriteLine("FETC:CHP:CHP?");
            double aux2 = Convert.ToDouble(Instr.ReadLine().Replace(".", ","));
            aux2 /= 1000;
            string val = Convert.ToString(aux2);
            var folderFile = await t;
            await SaveValueCSV(folderFile, $"PotenciaDeSaida", valFreq, val);
            if (tPrints)
            {
                await SaveFolderImage($"PotenciaDeSaida", valFreq, GetPrint(), folderFile);
            }
        }

        public async Task AssayDensidadeEspectralDePotencia(string tech, string user, string modu, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints, string alias, StorageFolder folder)
        {
            Start();
            var t = CreateCsv(tech, modu, $"DensidadeEspectralDePotencia", "N9010A", user, alias, folder);
            double Span = int.Parse(largura_Banda) * 1.5;
            ConfiguraInstr(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "1000", "3000", "ON", "AVER", "RMS", "SAN");
            Instr.WriteLine("SWE:TIME?");
            double tempo = Convert.ToDouble(Instr.ReadLine().Replace(".", ","));
            Thread.Sleep(15000);
            Thread.Sleep((int)tempo * 1000);
            var folderFile = await t;
            //await SaveValueCSV(folderFile, $"DensidadeEspectralDePotencia", valFreq);
            if (tPrints)
            {
                await SaveFolderImage($"DensidadeEspectralDePotencia", valFreq, GetPrint(), folderFile);
            }

        }

        internal async Task AssayEspurios(string tech, string user, string modu, string[] freq, string RefLevel, string Att)
        {
            Start();
            //var t = CreateCsv(tech, modu, $"Espurios", "N9010A", user, alias, folder);
            ConfiguraInstrSalto(freq[0], freq[1], "Dbm", Att, RefLevel, "100", "300", "ON", "MAXH", "POS", "SAN");
            Instr.WriteLine("INIT"); // Comece a varredura
            Thread.Sleep(5000);
            //var folderFile = await t;
            //SaveDbImage(modu, "Espurios", GetPrint(), "N9010A", user);

        }


        internal async Task AssaySeparacaoCanaisDeSalto(string tech, string user, string modu, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints, string alias, StorageFolder folder)
        {
            Start();
            double Span = int.Parse(largura_Banda) * 1.5;
            var t = CreateCsv(tech, modu, $"SeparacaoCanaisDeSalto", "N9010A", user, alias, folder);
            while (await WaitUser() == ContentDialogResult.Primary)
            {
                Start();
                ConfiguraInstr(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "100", "300", "ON", "MAXH", "POS", "SAN");
                Instr.WriteLine("INIT");
                Instr.WriteLine("INIT:CONT OFF");
                Thread.Sleep(2000);
            }
            List<string> val = GetMarker(2);
            string result = Convert.ToString(Math.Abs(Convert.ToInt32(val[0]) - Convert.ToInt32(val[1]))); 
            var folderFile = await t;
            await SaveValueCSV(folderFile, $"SeparacaoCanaisDeSalto", valFreq, result);
            if (tPrints)
            {
                await SaveFolderImage($"SeparacaoCanaisDeSalto", valFreq, GetPrint(), folderFile);
            }
            return;
        }


















        #endregion



        private void ConfiguraInstr(string freqC, string unidadeY, string att, string refL, string span, string rbw, string vbw, string sweepAuto, string trace, string detector, string modo)
        {
            Instr.WriteLine($"CONF:{modo}"); // Seleciona o modo
            Instr.WriteLine($"UNIT:POW {unidadeY}"); //Configura a unidade do reference Level
            Instr.WriteLine($"SENS:POW:RF:ATT {att} dB"); //Configura o ATT
            Instr.WriteLine($"DISP:WIND:TRAC:Y:SCAL:RLEV {refL} dbm"); // Configura o Reference Level
            Instr.WriteLine($"FREQ:CENT {freqC} Mhz"); // Configura a Frequencia Central
            Instr.WriteLine($"FREQ:SPAN {span} MHz"); // Configura o span
            Instr.WriteLine($"BAND {rbw} kHz"); // Configura o RBW
            Instr.WriteLine($"BAND:VID {vbw} kHz"); // Configura o VBW
            Instr.WriteLine($"SWE:TIME:AUTO {sweepAuto}"); // Configura o sweep points
            Instr.WriteLine($"TRAC:TYPE {trace}"); //Configura o Trace
            Instr.WriteLine($"SENS:DET:TRAC {detector}"); //Configura o Trace
        }

        private void Bandwidth(string freqC, string unidadeY, string att, string refL, string span, string rbw, string vbw, string sweepAuto, string trace, string detector, string modo, string porc_Ocu, string qDbs)
        {
            Instr.WriteLine($"CONF:{modo}"); // Seleciona o modo
            Instr.WriteLine($"SENS:OBW:PERC {porc_Ocu}"); // Seleciona a porcentagem de Occupied Bandwidth Measurement
            Instr.WriteLine($"SENS:OBW:XDB {qDbs} DB"); // Seleciona a distancia (6/20/26) db de Occupied Bandwidth Measurement
            Instr.WriteLine($"UNIT:POW {unidadeY}"); //Configura a unidade do reference Level
            Instr.WriteLine($"SENS:POW:RF:ATT {att} dB"); //Configura o ATT
            Instr.WriteLine($"DISP:OBW:VIEW:WIND:TRAC:Y:RLEV {refL} dbmw"); // Configura o Reference Level
            Instr.WriteLine($"FREQ:CENT {freqC} Mhz"); // Configura a Frequencia Central
            Instr.WriteLine($"OBW:FREQ:SPAN {span} Mhz"); // Configura o span
            Instr.WriteLine($"OBW:BWID:RES {rbw} kHz"); // Configura o RBW
            Instr.WriteLine($"OBW:BWID:VID {vbw} kHz"); // Configura o VBW
            Instr.WriteLine($"OBW:SWE:TIME:AUTO {sweepAuto}"); // Configura o sweep points
            Instr.WriteLine($"TRAC:OBW:TYPE {trace}"); //Configura o Trace
            Instr.WriteLine($"OBW:DET {detector}"); //Configura o Trace
        }

        private void ChannelPower(string freqC, string unidadeY, string att, string refL, string span, string rbw, string vbw, string sweepAuto, string trace, string detector, string modo, string integBW)
        {
            Instr.WriteLine($"CONF:{modo}"); // Seleciona o modo
            Instr.WriteLine($"CHP:BAND:INT {integBW} Mhz"); // Configura a largura de banda
            Instr.WriteLine($"UNIT:POW {unidadeY}"); //Configura a unidade do reference Level
            Instr.WriteLine($"SENS:POW:RF:ATT {att} dB"); //Configura o ATT
            Instr.WriteLine($"DISP:CHP:VIEW:WIND:TRAC:Y:RLEV {refL} dbmw"); // Configura o Reference Level
            Instr.WriteLine($"FREQ:CENT {freqC} Mhz"); // Configura a Frequencia Central
            Instr.WriteLine($"CHP:FREQ:SPAN {span} Mhz"); // Configura o span
            Instr.WriteLine($"CHP:BWID:RES {rbw} kHz"); // Configura o RBW
            Instr.WriteLine($"CHP:BWID:VID {vbw} kHz"); // Configura o VBW
            Instr.WriteLine($"CHP:SWE:TIME:AUTO {sweepAuto}"); // Configura o sweep points
            Instr.WriteLine($"TRAC:CHP:TYPE {trace}"); //Configura o Trace
            Instr.WriteLine($"CHP:DET {detector}"); //Configura o Trace
        }

        private void ConfiguraInstrSalto(string freqI, string freqF, string unidadeY, string att, string refL, string rbw, string vbw, string sweepAuto, string trace, string detector, string modo)
        {
            Instr.WriteLine($"CONF:{modo}"); // Seleciona o modo
            Instr.WriteLine($"UNIT:POW {unidadeY}"); //Configura a unidade do reference Level
            Instr.WriteLine($"SENS:POW:RF:ATT {att} dB"); //Configura o ATT
            Instr.WriteLine($"DISP:WIND:TRAC:Y:SCAL:RLEV {refL} dbm"); // Configura o Reference Level
            Instr.WriteLine($"FREQ:STAR {freqI} Mhz"); // Configura a Frequencia Inicial
            Instr.WriteLine($"FREQ:STOP {freqF} Mhz"); // Configura a Frequencia Final
            Instr.WriteLine($"BAND {rbw} kHz"); // Configura o RBW
            Instr.WriteLine($"BAND:VID {vbw} kHz"); // Configura o VBW
            Instr.WriteLine($"SWE:TIME:AUTO {sweepAuto}"); // Configura o sweep points
            Instr.WriteLine($"TRAC:TYPE {trace}"); //Configura o Trace
            Instr.WriteLine($"SENS:DET:TRAC {detector}"); //Configura o Trace
        }

        private void Start()
        {
            Instr.WriteLine("*RST;*CLS");
            Instr.WriteLine("INIT:CONT ON");
            Instr.WriteLine("DISP:ENAB ON");
        }

        private byte[] GetPrint()
        {
            Instr.WriteLine("HCOP:SDUM:DATA?");
            return Instr.ReadBytes();
        }

        private List<string> GetMarker(int count = 1)
        {
            List<string> result =  new List<string> { };
            double markerX = 1;
            double markerY = 1;
            double New_markerX = 0;
            double New_markerY = 0;
            for(int i= 1; i <= count; i++ )
            {
                while (markerY != New_markerY && markerX != New_markerX)
                {
                    markerX = New_markerX;
                    markerY = New_markerY;
                    Instr.WriteLine($"CALC1:MARK{i}:MAX"); //  Definindo o marker para o Peak search
                    Instr.WriteLine($"CALC1:MARK{i}:X?");
                    New_markerX = Convert.ToDouble(Instr.ReadLine().Replace(".", ","));
                    Instr.WriteLine($"CALC1:MARK{i}:Y?");
                    New_markerY = Convert.ToDouble(Instr.ReadLine().Replace(".", ","));
                    Thread.Sleep(10000);
                }
                result.Add(Convert.ToString(New_markerY));
            }
            
            return result;
        }

    }
}
