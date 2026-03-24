using System;
using MainSpecAn.Interfaces;
using MainSpecAn.SpecAn;

namespace MainSpecAn.Session
{
    /// <summary>
    /// Cria a implementação correta de ISpectrumAnalyzer conforme o fabricante.
    /// Para adicionar suporte ao ESR (R&amp;S): criar classe ESR : ISpectrumAnalyzer
    /// e registrar o case "RohdeSchwarz" aqui.
    /// </summary>
    public static class SpectrumAnalyzerFactory
    {
        private const int DefaultPort      = 5025;
        private const int DefaultTimeoutMs = 5000;

        public static ISpectrumAnalyzer Create(string brand, string ip,
            int port = DefaultPort, int timeoutMs = DefaultTimeoutMs)
        {
            return brand switch
            {
                "Keysight" => new N9010A(ip, port, timeoutMs),
                _ => throw new NotSupportedException(
                         $"Instrumento '{brand}' não suportado. " +
                         "Marcas disponíveis: Keysight")
            };
        }
    }
}
