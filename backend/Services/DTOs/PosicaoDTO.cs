namespace InvestApp.Services.DTOs
{
    public class PosicaoDTO
    {
        public int UsuarioId { get; set; }
        public int AtivoId { get; set; }
        public int QuantidadeLiquida { get; set; }
        public decimal PrecoMedio { get; set; }
        public decimal ValorMercado { get; set; }
        public decimal PnL { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
    }
} 