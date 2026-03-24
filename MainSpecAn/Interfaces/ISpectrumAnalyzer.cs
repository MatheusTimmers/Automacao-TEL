using System.Collections.Generic;

namespace MainSpecAn.Interfaces
{
    /// <summary>
    /// Contrato comum para comunicação com analisadores de espectro via SCPI.
    /// Implementações: N9010A (Keysight), ESR (Rohde &amp; Schwarz), etc.
    /// </summary>
    public interface ISpectrumAnalyzer
    {
        // ── Ciclo de vida ──────────────────────────────────────────────────────
        /// <summary>Reset completo (*RST;*CLS) e habilita display.</summary>
        void Reset();

        /// <summary>Liga ou desliga varredura contínua (INIT:CONT).</summary>
        void SetContinuousSweep(bool continuous);

        /// <summary>Dispara uma varredura (INIT).</summary>
        void InitiateSweep();

        // ── Configuração de modos de medição ───────────────────────────────────
        /// <summary>Configura o modo Span Analyzer (SAN).</summary>
        void ConfigureSpanAnalyzer(MeasurementConfig config);

        /// <summary>Configura o modo Occupied Bandwidth (OBW).</summary>
        /// <param name="occupancyPercent">Ex: "99"</param>
        /// <param name="xDbDown">Ex: "-6", "-26"</param>
        void ConfigureOBW(MeasurementConfig config, string occupancyPercent, string xDbDown);

        /// <summary>Configura o modo Channel Power (CHP).</summary>
        /// <param name="integrationBandwidthMHz">Largura de banda de integração em MHz.</param>
        void ConfigureChannelPower(MeasurementConfig config, string integrationBandwidthMHz);

        /// <summary>Configura varredura por faixa de frequência (para espúrios).</summary>
        void ConfigureFrequencyRange(string startFreqMHz, string stopFreqMHz, MeasurementConfig config);

        // ── Leituras ──────────────────────────────────────────────────────────
        /// <summary>Retorna o tempo de varredura atual em segundos.</summary>
        double GetSweepTime();

        /// <summary>Retorna o resultado da medição OBW em Hz.</summary>
        double FetchOBWResult();

        /// <summary>Retorna o resultado de Channel Power em dBm.</summary>
        double FetchChannelPowerResult();

        /// <summary>Retorna os valores Y (dBm) dos markers de pico.</summary>
        List<double> GetPeakMarkers(int count = 1);

        // ── Utilitários ───────────────────────────────────────────────────────
        /// <summary>Captura a tela do instrumento e retorna os bytes da imagem PNG.</summary>
        byte[] CaptureScreen();
    }
}
