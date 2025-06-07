namespace InvestApp.Services.DTOs
{
    public class GlobalPositionDto
    {
        public int UsuarioId { get; set; }
        public decimal ValorMercado { get; set; }
        public decimal CustoTotal { get; set; }
        public decimal PnL { get; set; }
        public List<OperacaoDTO> Operacoes { get; set; } = new();
        public List<HistoricoCarteiraDTO> HistoricoCarteira { get; set; } = new();
        public List<DistribuicaoAtivoDTO> DistribuicaoAtivos { get; set; } = new();
    }

    public class HistoricoCarteiraDTO
    {
        public string Data { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }

    public class DistribuicaoAtivoDTO
    {
        public string Ativo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
} 