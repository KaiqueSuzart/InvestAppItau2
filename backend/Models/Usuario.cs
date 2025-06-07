using System;

namespace InvestApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public decimal PercCorretagem { get; set; }
        public string Senha { get; set; }

        public Usuario()
        {
            Nome = string.Empty;
            Email = string.Empty;
        }
    }
} 