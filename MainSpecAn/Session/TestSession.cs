using Windows.Storage;
using MainSpecAn.Interfaces;

namespace MainSpecAn.Session
{
    /// <summary>
    /// Contém o estado da sessão de ensaios: conexão com o instrumento,
    /// pasta de saída e identificação do DUT.
    /// Substituí o papel de estado global que MainAssay cumpria antes.
    /// </summary>
    public class TestSession
    {
        public string           Ip          { get; private set; } = "";
        public bool             IsConnected { get; private set; }
        public StorageFolder    OutputFolder { get; set; }
        public string           Alias        { get; set; }
        public ISpectrumAnalyzer Instrument  { get; private set; }

        /// <summary>
        /// Conecta ao instrumento. Lança SocketException se inacessível.
        /// </summary>
        /// <param name="ip">Endereço IP do analisador.</param>
        /// <param name="brand">Fabricante: "Keysight" (padrão).</param>
        public void Connect(string ip, string brand = "Keysight")
        {
            Instrument  = SpectrumAnalyzerFactory.Create(brand, ip);
            Ip          = ip;
            IsConnected = true;
        }

        public void Disconnect()
        {
            Instrument  = null;
            IsConnected = false;
        }
    }
}
