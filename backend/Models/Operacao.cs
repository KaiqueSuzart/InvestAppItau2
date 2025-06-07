using System;

namespace InvestApp.Models
{
    public class Operacao
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int AtivoId { get; set; }
        public string TipoOperacao { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Corretagem { get; set; }
        public DateTime DataHora { get; set; }
        public string CodigoAtivo { get; set; } = string.Empty;

        public Operacao()
        {
            TipoOperacao = string.Empty;
        }
    }
} 