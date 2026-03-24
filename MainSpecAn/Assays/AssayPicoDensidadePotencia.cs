using System.Globalization;
using System.Threading.Tasks;
using MainSpecAn.Interfaces;

namespace MainSpecAn.Assays
{
    /// <summary>
    /// Pico da Densidade de Potência (Span Analyzer, marker de pico, trace MAXH, detector POS).
    /// </summary>
    public class AssayPicoDensidadePotencia
    {
        private const int SweepWaitMs = 15_000;

        private readonly ISpectrumAnalyzer _instrument;

        public AssayPicoDensidadePotencia(ISpectrumAnalyzer instrument)
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
                RbwKHz             = "3",
                VbwKHz             = "10",
                AutoSweep          = true,
                TraceMode          = "MAXH",
                Detector           = "POS"
            };

            _instrument.Reset();
            _instrument.ConfigureSpanAnalyzer(config);
            _instrument.InitiateSweep();

            await Task.Delay(SweepWaitMs);

            var markers = _instrument.GetPeakMarkers(1);
            double value = markers[0];

            byte[] screenshot = captureScreen ? _instrument.CaptureScreen() : null;

            return new AssayResult { Value = value, Unit = "dBm/Hz", Screenshot = screenshot };
        }
    }
}
