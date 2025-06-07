namespace InvestApp.Services.DTOs
{
    public class TotalInvestidoPorAtivoDto
    {
        public int AtivoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public decimal TotalInvestido { get; set; }
    }
} 