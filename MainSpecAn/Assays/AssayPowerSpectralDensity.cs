using System;
using System.Globalization;
using System.Threading.Tasks;
using MainSpecAn.Interfaces;

namespace MainSpecAn.Assays
{
    /// <summary>
    /// Densidade Espectral de Potência (Span Analyzer, AVER, RMS).
    /// Ensaio visual — captura apenas a tela, sem valor numérico.
    /// </summary>
    public class AssayPowerSpectralDensity
    {
        private const int BaseWaitMs = 15_000;

        private readonly ISpectrumAnalyzer _instrument;

        public AssayPowerSpectralDensity(ISpectrumAnalyzer instrument)
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

            var config = new MeasurementConfig
            {
                CenterFrequencyMHz = centerFreqMHz,
                AttenuationDb      = attDb,
                ReferenceLevelDbm  = refLevelDbm,
                SpanMHz            = span.ToString(CultureInfo.InvariantCulture),
                RbwKHz             = "1000",
                VbwKHz             = "3000",
                AutoSweep          = true,
                TraceMode          = "AVER",
                Detector           = "RMS"
            };

            _instrument.Reset();
            _instrument.ConfigureSpanAnalyzer(config);
            _instrument.InitiateSweep();

            double sweepTimeSec = _instrument.GetSweepTime();
            int totalWaitMs = BaseWaitMs + (int)(sweepTimeSec * 1_000);
            await Task.Delay(totalWaitMs);

            byte[] screenshot = captureScreen ? _instrument.CaptureScreen() : null;

            // Ensaio visual — sem valor numérico
            return new AssayResult { Value = null, Unit = null, Screenshot = screenshot };
        }
    }
}
