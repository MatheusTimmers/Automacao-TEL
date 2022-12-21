using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConnectLan;
using System.Drawing;

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
        internal string AssayLarguraDeBanda(string user, string alias, string valFreq, string ndbDown, string bandwidth, string RefLevel, string Att, bool tPrints)
        {
            try
            {
                Start();
                double Span = int.Parse(bandwidth) * 1.5;
                Bandwidth(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "100", "300", "ON", "MAXH", "POS", "OBW", "99", "-" + ndbDown);
                Instr.WriteLine("INIT");
                Thread.Sleep(15000);
                Instr.WriteLine("INIT:CONT OFF");
                Instr.WriteLine("FETC:OBW:XDB?");
                double aux = Convert.ToDouble(Instr.ReadLine());
                aux /= 1000;
                string val = Convert.ToString(aux);
                SaveDb(alias, valFreq, val, $"Largura de Banda a {ndbDown}", "N9010A", user);
                if (tPrints)
                {
                    SaveDbImage(alias, $"Largura de Banda a {ndbDown}", GetPrint(), "N9010A", user);
                }
                return "Ok";

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        internal string AssayPicoDaDensidadeDePotencia(string alias, string user, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints)
        {
            try
            {
                Start();
                double Span = int.Parse(largura_Banda) * 1.5;
                ConfiguraInstr(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "3", "10", "ON", "MAXH", "POS", "SAN");
                Instr.WriteLine("INIT");
                Thread.Sleep(15000);
                string val = GetMarker();
                SaveDb(alias, valFreq, val, $"PicoDaDensidadeDePotencia", "N9010A", user);
                if (tPrints)
                {
                    SaveDbImage(alias, "PicoDaDensidadeDePotencia", GetPrint(), "N9010A", user);
                }
                return "Ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        internal string AssayValorMedioDensidadeEspectral(string user, string alias, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints)
        {
            try
            {
                Start();
                double Span = int.Parse(largura_Banda) * 1.5;
                ConfiguraInstr(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "3", "10", "ON", "AVER", "RMS", "SAN");
                Instr.WriteLine("INIT");
                Instr.WriteLine("SWE:TIME?");
                double tempo = Convert.ToDouble(Instr.ReadLine());
                Thread.Sleep(10000);
                Thread.Sleep((int)tempo * 1000);
                string val = GetMarker();
                SaveDb(alias, valFreq, val, $"ValorMedioDensidadeEspectral", "N9010A", user);
                if (tPrints)
                {
                    SaveDbImage(alias, "ValorMedioDensidadeEspectral", GetPrint(), "N9010A", user);
                }
                return "Ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        internal string AssayPotenciaDePicoMaxima(string user, string alias, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints)
        {
            try
            {
                Start();
                double Span = int.Parse(largura_Banda) * 1.5;
                ChannelPower(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "1000", "3000", "ON", "MAXH", "POS", "CHP", largura_Banda);
                Instr.WriteLine("INIT");
                Thread.Sleep(15000);
                Instr.WriteLine("INIT:CONT OFF");
                Instr.WriteLine("FETC:CHP:CHP?");
                string val = Instr.ReadLine();
                SaveDb(alias, valFreq, val, $"PotenciaDePicoMaxima", "N9010A", user);
                if (tPrints)
                {
                    SaveDbImage(alias, "PotenciaDePicoMaxima", GetPrint(), "N9010A", user);
                }
                return "Ok";

            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        internal string AssayValorMedioDaPotenciaMaximaDeSaida(string user, string alias, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints)
        {
            try
            {
                Start();
                double Span = int.Parse(largura_Banda) * 1.5;
                ChannelPower(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "1000", "3000", "ON", "MAXH", "RMS", "CHP", largura_Banda);
                Instr.WriteLine("INIT");
                Thread.Sleep(15000);
                Instr.WriteLine("INIT:CONT OFF");
                Instr.WriteLine("FETC:CHP:CHP?");
                double aux = Convert.ToDouble(Instr.ReadLine());
                aux /= 1000;
                string val = Convert.ToString(aux);
                SaveDb(alias, valFreq, val, $"ValorMedioDaPotenciaMaximaDeSaida", "N9010A", user);
                if (tPrints)
                {
                    SaveDbImage(alias, "ValorMedioDaPotenciaMaximaDeSaida", GetPrint(),"N9010A", user);
                }
                return "Ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public string AssayPotenciaDeSaida(string user, string alias, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints)
        {
            try
            {
                Start();
                double Span = int.Parse(largura_Banda) * 1.5;

                Bandwidth(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "100", "300", "ON", "MAXH", "POS", "OBW", "99", "-26");
                Instr.WriteLine("INIT");
                Thread.Sleep(15000);
                Instr.WriteLine("INIT:CONT OFF");
                Instr.WriteLine("FETC:OBW:XDB?");
                double aux = Convert.ToDouble(Instr.ReadLine());
                aux /= 1000000;
                string aux1 = Convert.ToString(aux);
                ChannelPower(valFreq, "DBM", Att, RefLevel, Span.ToString(), "1000", "3000", "ON", "AVER", "RMS", "CHP", aux1);
                Instr.WriteLine("INIT");
                Thread.Sleep(15000);
                Instr.WriteLine("INIT:CONT OFF");
                Instr.WriteLine("FETC:CHP:CHP?");
                double aux2 = Convert.ToDouble(Instr.ReadLine());
                aux2 /= 1000;
                string val = Convert.ToString(aux2);
                SaveDb(alias, valFreq, val, $"PotenciaDeSaida", "N9010A", user);
                if (tPrints)
                {
                    SaveDbImage(alias, "PotenciaDeSaida", GetPrint(), "N9010A", user);
                }
                return "Ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string AssayDensidadeEspectralDePotencia(string user, string alias, string valFreq, string largura_Banda, string RefLevel, string Att, bool tPrints)
        {
            try
            {
                Start();
                double Span = int.Parse(largura_Banda) * 1.5;
                ConfiguraInstr(valFreq, "Dbm", Att, RefLevel, Span.ToString(), "1000", "3000", "ON", "AVER", "RMS", "SAN");
                Instr.WriteLine("SWE:TIME?");
                double tempo = Convert.ToDouble(Instr.ReadLine());
                Thread.Sleep(15000);
                Thread.Sleep((int)tempo * 1000);
                SaveDb(alias, valFreq, GetMarker(), $"Pico Da Densidade De Potencia", "N9010A", user);
                if (tPrints)
                {
                    SaveDbImage(alias, "DensidadeEspectralDePotencia", GetPrint(), "N9010A", user);
                }
                return "Ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        internal string AssayEspurios(string user, string alias, string[] freq, string RefLevel, string Att)
        {
            try
            {
                Start();
                ConfiguraInstrSalto(freq[0], freq[1], "Dbm", Att, RefLevel, "100", "300", "ON", "MAXH", "POS", "SAN");
                Instr.WriteLine("INIT"); // Comece a varredura
                Thread.Sleep(5000);
                SaveDbImage(alias, "Espurios", GetPrint(), "N9010A", user);
                return "Ok";
            }
            catch (Exception e)
            {
                return e.Message;
            }
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
            Instr.WriteLine("HCOP:SDUM?");
            return Instr.ReadBytes();
        }


        private string GetMarker()
        {
            double markerX = 1;
            double markerY = 1;
            double New_markerX = 0;
            double New_markerY = 0;
            while (markerY != New_markerY && markerX != New_markerX)
            {
                markerX = New_markerX;
                markerY = New_markerY;
                Instr.WriteLine("CALC1:MARK1:MAX"); //  Definindo o marker para o Peak search
                Instr.WriteLine("CALC1:MARK1:X?");
                New_markerX = Convert.ToDouble(Instr.ReadLine());
                Instr.WriteLine("CALC1:MARK1:Y?");
                New_markerY = Convert.ToDouble(Instr.ReadLine());
                Thread.Sleep(10000);
            }
            return Convert.ToString(New_markerY);
        }



    }
}
