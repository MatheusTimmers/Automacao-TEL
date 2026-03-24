using System.Globalization;
using System.Threading.Tasks;
using MainSpecAn.Interfaces;

namespace MainSpecAn.Assays
{
    /// <summary>
    /// Largura de Faixa (OBW) a N dB.
    /// Norma 14448 Anatel — itens aplicáveis a WIFI e Bluetooth.
    /// </summary>
    public class AssayLarguraDeBanda
    {
        private const int SweepWaitMs = 15_000;

        private readonly ISpectrumAnalyzer _instrument;

        public AssayLarguraDeBanda(ISpectrumAnalyzer instrument)
        {
            _instrument = instrument;
        }

        /// <param name="centerFreqMHz">Frequência central em MHz.</param>
        /// <param name="bandwidthMHz">Largura de banda nominal em MHz (span = 1.5x).</param>
        /// <param name="refLevelDbm">Nível de referência em dBm.</param>
        /// <param name="attDb">Atenuação em dB.</param>
        /// <param name="xDbDown">Ex: "6", "20", "26" — dB abaixo do pico.</param>
        /// <param name="captureScreen">Capturar tela do instrumento ao final.</param>
        public async Task<AssayResult> ExecuteAsync(
            string centerFreqMHz,
            string bandwidthMHz,
            string refLevelDbm,
            string attDb,
            string xDbDown,
            bool   captureScreen)
        {
            double span = double.Parse(bandwidthMHz, CultureInfo.InvariantCulture) * 1.5;

            var config = new MeasurementConfig
            {
                CenterFrequencyMHz = centerFreqMHz,
                AttenuationDb      = attDb,
                ReferenceLevelDbm  = refLevelDbm,
                SpanMHz            = span.ToString(CultureInfo.InvariantCulture),
                RbwKHz             = "100",
                VbwKHz             = "300",
                AutoSweep          = true,
                TraceMode          = "MAXH",
                Detector           = "POS"
            };

            _instrument.Reset();
            _instrument.ConfigureOBW(config, "99", "-" + xDbDown);
            _instrument.InitiateSweep();

            await Task.Delay(SweepWaitMs);

            _instrument.SetContinuousSweep(false);
            double valueHz  = _instrument.FetchOBWResult();
            double valueKHz = valueHz / 1_000.0;

            byte[] screenshot = captureScreen ? _instrument.CaptureScreen() : null;

            return new AssayResult { Value = valueKHz, Unit = "kHz", Screenshot = screenshot };
        }
    }
}
