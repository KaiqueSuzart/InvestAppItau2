using System;

namespace InvestApp.Models
{
    public class Cotacao
    {
        public int Id { get; set; }
        public int AtivoId { get; set; }
        public decimal PrecoUnitario { get; set; }
        public DateTime DataHora { get; set; }
    }
} 