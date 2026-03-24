using System;
using System.Globalization;
using System.Threading.Tasks;
using MainSpecAn.Interfaces;

namespace MainSpecAn.Assays
{
    /// <summary>
    /// Valor Médio da Densidade Espectral de Potência
    /// (Span Analyzer, trace AVER, detector RMS).
    /// Aguarda varredura completa antes de ler o marker.
    /// </summary>
    public class AssayValorMedioDensidadeEspectral
    {
        private const int BaseWaitMs = 10_000;

        private readonly ISpectrumAnalyzer _instrument;

        public AssayValorMedioDensidadeEspectral(ISpectrumAnalyzer instrument)
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
                TraceMode          = "AVER",
                Detector           = "RMS"
            };

            _instrument.Reset();
            _instrument.ConfigureSpanAnalyzer(config);
            _instrument.InitiateSweep();

            double sweepTimeSec = _instrument.GetSweepTime();
            int totalWaitMs = BaseWaitMs + (int)(sweepTimeSec * 1_000);
            await Task.Delay(totalWaitMs);

            var markers = _instrument.GetPeakMarkers(1);
            double value = markers[0];

            byte[] screenshot = captureScreen ? _instrument.CaptureScreen() : null;

            return new AssayResult { Value = value, Unit = "dBm/Hz", Screenshot = screenshot };
        }
    }
}
