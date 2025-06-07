using System;

namespace InvestApp.Models
{
    public class Ativo
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public decimal PrecoAtual { get; set; }

        public Ativo()
        {
            Codigo = string.Empty;
            Nome = string.Empty;
        }
    }
} 