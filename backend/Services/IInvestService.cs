using System.Collections.Generic;
using System.Threading.Tasks;
using InvestApp.Services.DTOs;
using InvestApp.Models;

namespace InvestApp.Services
{
    public interface IInvestService
    {
        Task<Usuario?> GetUsuarioAsync(int id);
        Task<IEnumerable<TotalInvestidoPorAtivoDto>> GetTotalInvestidoPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<PosicaoPorPapelDto>> GetPosicaoPorPapelAsync(int usuarioId);
        Task<GlobalPositionDto> GetPosicaoGlobalComPnlAsync(int usuarioId);
        Task<decimal> GetTotalCorretagemPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<Operacao>> GetOperacoesPorUsuarioAsync(int usuarioId);
        Task<decimal?> GetUltimaCotacaoAteDataAsync(int ativoId, DateTime data);
        Task<IEnumerable<Cotacao>> GetCotacoesPorAtivoAsync(int ativoId);
        Task InserirCotacaoAsync(int ativoId, decimal precoUnitario, DateTime dataHora);
        Task UpdateOperacaoAsync(int id, OperacaoDTO dto);
        Task<Ativo?> GetAtivoPorCodigoAsync(string codigo);
        Task InserirOperacaoAsync(Operacao operacao);
    }
} 