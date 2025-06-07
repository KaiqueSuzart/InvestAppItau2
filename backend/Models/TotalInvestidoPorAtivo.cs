namespace InvestApp.Models;

public class TotalInvestidoPorAtivo
{
    public int AtivoId { get; set; }
    public string Codigo { get; set; }
    public string Nome { get; set; }
    public decimal TotalInvestido { get; set; }

    public TotalInvestidoPorAtivo()
    {
        Codigo = string.Empty;
        Nome = string.Empty;
    }
} 