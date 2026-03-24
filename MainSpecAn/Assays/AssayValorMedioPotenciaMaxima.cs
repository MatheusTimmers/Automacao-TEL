using System.Globalization;
using System.Threading.Tasks;
using MainSpecAn.Interfaces;

namespace MainSpecAn.Assays
{
    /// <summary>
    /// Valor Médio da Potência Máxima de Saída (Channel Power, detector RMS, trace MAXH).
    /// </summary>
    public class AssayValorMedioPotenciaMaxima
    {
        private const int SweepWaitMs = 15_000;

        private readonly ISpectrumAnalyzer _instrument;

        public AssayValorMedioPotenciaMaxima(ISpectrumAnalyzer instrument)
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
                Detector           = "RMS"
            };

            _instrument.Reset();
            _instrument.ConfigureChannelPower(config, bandwidthMHz);
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
