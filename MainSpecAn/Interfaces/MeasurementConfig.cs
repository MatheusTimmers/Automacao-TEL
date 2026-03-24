namespace MainSpecAn.Interfaces
{
    /// <summary>
    /// Parâmetros comuns de configuração do analisador de espectro para um ensaio.
    /// </summary>
    public class MeasurementConfig
    {
        public string CenterFrequencyMHz  { get; set; }
        public string AttenuationDb       { get; set; }
        public string ReferenceLevelDbm   { get; set; }
        public string SpanMHz             { get; set; }
        public string RbwKHz              { get; set; }
        public string VbwKHz              { get; set; }
        public bool   AutoSweep           { get; set; } = true;
        public string TraceMode           { get; set; }  // "MAXH" | "AVER"
        public string Detector            { get; set; }  // "POS"  | "RMS"
        public string PowerUnit           { get; set; } = "Dbm";
    }
}
