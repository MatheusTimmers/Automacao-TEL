using System;
using System.Globalization;
using System.Threading.Tasks;
using MainSpecAn.Interfaces;

namespace MainSpecAn.Assays
{
    /// <summary>
    /// Potência de Saída:
    /// 1. Mede OBW a -26 dB para obter a largura de banda real.
    /// 2. Usa esse valor como integration bandwidth no Channel Power (AVER, RMS).
    /// </summary>
    public class AssayOutputPower
    {
        private const int SweepWaitMs = 15_000;

        private readonly ISpectrumAnalyzer _instrument;

        public AssayOutputPower(ISpectrumAnalyzer instrument)
        {
            _instrument = instrument;
        }

        public async Task<AssayResult> ExecuteAsync(
            string centerFreqMHz,
            string bandwidthMHz,
            string refLevelDbm,
            string attDb,
            bool   captureScreen)
        {
            double span = double.Parse(bandwidthMHz, CultureInfo.InvariantCulture) * 1.5;
            string spanStr = span.ToString(CultureInfo.InvariantCulture);

            // ── Passo 1: medir OBW a -26 dB para obter largura de banda real ──
            var obwConfig = new MeasurementConfig
            {
                CenterFrequencyMHz = centerFreqMHz,
                AttenuationDb      = attDb,
                ReferenceLevelDbm  = refLevelDbm,
                SpanMHz            = spanStr,
                RbwKHz             = "100",
                VbwKHz             = "300",
                AutoSweep          = true,
                TraceMode          = "MAXH",
                Detector           = "POS"
            };

            _instrument.Reset();
            _instrument.ConfigureOBW(obwConfig, "99", "-26");
            _instrument.InitiateSweep();

            await Task.Delay(SweepWaitMs);

            _instrument.SetContinuousSweep(false);
            double obwHz  = _instrument.FetchOBWResult();
            double obwMHz = obwHz / 1_000_000.0;  // Hz → MHz (integration BW em MHz)
            string integBWStr = obwMHz.ToString(CultureInfo.InvariantCulture);

            // ── Passo 2: Channel Power usando a largura medida ─────────────────
            var chpConfig = new MeasurementConfig
            {
                CenterFrequencyMHz = centerFreqMHz,
                AttenuationDb      = attDb,
                ReferenceLevelDbm  = refLevelDbm,
                SpanMHz            = spanStr,
                RbwKHz             = "1000",
                VbwKHz             = "3000",
                AutoSweep          = true,
                TraceMode          = "AVER",
                Detector           = "RMS"
            };

            _instrument.Reset();
            _instrument.ConfigureChannelPower(chpConfig, integBWStr);
            _instrument.InitiateSweep();

            await Task.Delay(SweepWaitMs);

            _instrument.SetContinuousSweep(false);
            double rawValue  = _instrument.FetchChannelPowerResult();
            double valueKHz  = rawValue / 1_000.0;

            byte[] screenshot = captureScreen ? _instrument.CaptureScreen() : null;

            return new AssayResult { Value = valueKHz, Unit = "kHz", Screenshot = screenshot };
        }
    }
}
