namespace InvestApp.Services.DTOs
{
    public class PosicaoPorPapelDto
    {
        public int AtivoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public decimal QuantidadeLiquida { get; set; }
        public decimal PrecoMedio { get; set; }
        public decimal PrecoAtual { get; set; }
    }
} 