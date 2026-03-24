using System.Globalization;
using System.Threading.Tasks;
using MainSpecAn.Interfaces;

namespace MainSpecAn.Assays
{
    /// <summary>
    /// Potência de Pico Máxima (Channel Power, detector POS, trace MAXH).
    /// </summary>
    public class AssayMaximumPeakPower
    {
        private const int SweepWaitMs = 15_000;

        private readonly ISpectrumAnalyzer _instrument;

        public AssayMaximumPeakPower(ISpectrumAnalyzer instrument)
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
                TraceMode          = "MAXH",
                Detector           = "POS"
            };

            _instrument.Reset();
            _instrument.ConfigureChannelPower(config, bandwidthMHz);
            _instrument.InitiateSweep();

            await Task.Delay(SweepWaitMs);

            _instrument.SetContinuousSweep(false);
            double value = _instrument.FetchChannelPowerResult();

            byte[] screenshot = captureScreen ? _instrument.CaptureScreen() : null;

            return new AssayResult { Value = value, Unit = "dBm", Screenshot = screenshot };
        }
    }
}
