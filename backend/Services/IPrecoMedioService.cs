using InvestApp.Services.DTOs;

namespace InvestApp.Services
{
    public interface IPrecoMedioService
    {
        /// <summary>
        /// Calcula o preço médio ponderado das operações de compra
        /// </summary>
        /// <param name="operacoes">Lista de operações de compra</param>
        /// <returns>Preço médio ponderado</returns>
        /// <exception cref="ArgumentException">Lançada quando a lista de operações é nula, vazia ou contém operações inválidas</exception>
        decimal CalcularPrecoMedioPonderado(List<OperacaoDTO> operacoes);
    }
} 