namespace InvestApp.Models;

public class PosicaoPorPapel
{
    public int AtivoId { get; set; }
    public string Codigo { get; set; }
    public string Nome { get; set; }
    public decimal QuantidadeLiquida { get; set; }
    public decimal PrecoMedio { get; set; }
    public decimal PrecoAtual { get; set; }

    public PosicaoPorPapel()
    {
        Codigo = string.Empty;
        Nome = string.Empty;
    }
} 