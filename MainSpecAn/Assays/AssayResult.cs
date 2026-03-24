namespace MainSpecAn.Assays
{
    /// <summary>
    /// Resultado de um ensaio individual.
    /// Value é null para ensaios que capturam apenas a tela (sem valor numérico).
    /// </summary>
    public class AssayResult
    {
        public double? Value      { get; set; }
        public string  Unit       { get; set; }
        public byte[]  Screenshot { get; set; }
    }
}
