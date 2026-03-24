using System;
using System.Collections.Generic;
using System.Globalization;
using ConnectLan;
using MainSpecAn.Interfaces;

namespace MainSpecAn.SpecAn
{
    /// <summary>
    /// Driver SCPI para o analisador de espectro Keysight N9010A.
    /// Responsabilidade exclusiva: traduzir operações da interface ISpectrumAnalyzer
    /// em comandos SCPI enviados via TCP (porta 5025).
    /// A orquestração dos ensaios (esperas, lógica de medição) fica nas classes de ensaio.
    /// </summary>
    internal class N9010A : ISpectrumAnalyzer
    {
        private readonly Connect _connect;

        public N9010A(string instrIPAddress, int instrPortNo, int timeOut)
        {
            _connect = new Connect(instrIPAddress, instrPortNo, timeOut);
        }

        // ── Ciclo de vida ──────────────────────────────────────────────────────

        public void Reset()
        {
            Send("*RST;*CLS");
            Send("INIT:CONT ON");
            Send("DISP:ENAB ON");
        }

        public void SetContinuousSweep(bool continuous)
        {
            Send($"INIT:CONT {(continuous ? "ON" : "OFF")}");
        }

        public void InitiateSweep()
        {
            Send("INIT");
        }

        // ── Configuração de modos ──────────────────────────────────────────────

        public void ConfigureSpanAnalyzer(MeasurementConfig c)
        {
            Send($"CONF:SAN");
            Send($"UNIT:POW {c.PowerUnit}");
            Send($"SENS:POW:RF:ATT {c.AttenuationDb} dB");
            Send($"DISP:WIND:TRAC:Y:SCAL:RLEV {c.ReferenceLevelDbm} dbm");
            Send($"FREQ:CENT {c.CenterFrequencyMHz} Mhz");
            Send($"FREQ:SPAN {c.SpanMHz} MHz");
            Send($"BAND {c.RbwKHz} kHz");
            Send($"BAND:VID {c.VbwKHz} kHz");
            Send($"SWE:TIME:AUTO {SweepAuto(c)}");
            Send($"TRAC:TYPE {c.TraceMode}");
            Send($"SENS:DET:TRAC {c.Detector}");
        }

        public void ConfigureOBW(MeasurementConfig c, string occupancyPercent, string xDbDown)
        {
            Send($"CONF:OBW");
            Send($"SENS:OBW:PERC {occupancyPercent}");
            Send($"SENS:OBW:XDB {xDbDown} DB");
            Send($"UNIT:POW {c.PowerUnit}");
            Send($"SENS:POW:RF:ATT {c.AttenuationDb} dB");
            Send($"DISP:OBW:VIEW:WIND:TRAC:Y:RLEV {c.ReferenceLevelDbm} dbmw");
            Send($"FREQ:CENT {c.CenterFrequencyMHz} Mhz");
            Send($"OBW:FREQ:SPAN {c.SpanMHz} Mhz");
            Send($"OBW:BWID:RES {c.RbwKHz} kHz");
            Send($"OBW:BWID:VID {c.VbwKHz} kHz");
            Send($"OBW:SWE:TIME:AUTO {SweepAuto(c)}");
            Send($"TRAC:OBW:TYPE {c.TraceMode}");
            Send($"OBW:DET {c.Detector}");
        }

        public void ConfigureChannelPower(MeasurementConfig c, string integrationBandwidthMHz)
        {
            Send($"CONF:CHP");
            Send($"CHP:BAND:INT {integrationBandwidthMHz} Mhz");
            Send($"UNIT:POW {c.PowerUnit}");
            Send($"SENS:POW:RF:ATT {c.AttenuationDb} dB");
            Send($"DISP:CHP:VIEW:WIND:TRAC:Y:RLEV {c.ReferenceLevelDbm} dbmw");
            Send($"FREQ:CENT {c.CenterFrequencyMHz} Mhz");
            Send($"CHP:FREQ:SPAN {c.SpanMHz} Mhz");
            Send($"CHP:BWID:RES {c.RbwKHz} kHz");
            Send($"CHP:BWID:VID {c.VbwKHz} kHz");
            Send($"CHP:SWE:TIME:AUTO {SweepAuto(c)}");
            Send($"TRAC:CHP:TYPE {c.TraceMode}");
            Send($"CHP:DET {c.Detector}");
        }

        public void ConfigureFrequencyRange(string startFreqMHz, string stopFreqMHz, MeasurementConfig c)
        {
            Send($"CONF:SAN");
            Send($"UNIT:POW {c.PowerUnit}");
            Send($"SENS:POW:RF:ATT {c.AttenuationDb} dB");
            Send($"DISP:WIND:TRAC:Y:SCAL:RLEV {c.ReferenceLevelDbm} dbm");
            Send($"FREQ:STAR {startFreqMHz} Mhz");
            Send($"FREQ:STOP {stopFreqMHz} Mhz");
            Send($"BAND {c.RbwKHz} kHz");
            Send($"BAND:VID {c.VbwKHz} kHz");
            Send($"SWE:TIME:AUTO {SweepAuto(c)}");
            Send($"TRAC:TYPE {c.TraceMode}");
            Send($"SENS:DET:TRAC {c.Detector}");
        }

        // ── Leituras ──────────────────────────────────────────────────────────

        public double GetSweepTime()
        {
            Send("SWE:TIME?");
            return ParseDouble(Read());
        }

        public double FetchOBWResult()
        {
            Send("FETC:OBW:XDB?");
            return ParseDouble(Read());
        }

        public double FetchChannelPowerResult()
        {
            Send("FETC:CHP:CHP?");
            return ParseDouble(Read());
        }

        public List<double> GetPeakMarkers(int count = 1)
        {
            var result = new List<double>();
            for (int i = 1; i <= count; i++)
            {
                Send($"CALC1:MARK{i}:MAX");
                Send($"CALC1:MARK{i}:Y?");
                result.Add(ParseDouble(Read()));
            }
            return result;
        }

        // ── Utilitários ───────────────────────────────────────────────────────

        public byte[] CaptureScreen()
        {
            Send("HCOP:SDUM:DATA?");
            return _connect.ReadBytes();
        }

        // ── Helpers privados ──────────────────────────────────────────────────

        private void Send(string command) => _connect.WriteLine(command);

        private string Read() => _connect.ReadLine();

        private static string SweepAuto(MeasurementConfig c) => c.AutoSweep ? "ON" : "OFF";

        private static double ParseDouble(string value) =>
            double.Parse(value.Trim(), CultureInfo.InvariantCulture);
    }
}
