namespace InvestApp.Services.DTOs
{
    public class OperacaoDTO
    {
        /// <summary>
        /// Quantidade de ações na operação
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Preço unitário da ação na operação
        /// </summary>
        public decimal Preco { get; set; }

        /// <summary>
        /// Tipo da operação (COMPRA ou VENDA)
        /// </summary>
        public string TipoOperacao { get; set; } = string.Empty;

        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Ativo { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }
} 