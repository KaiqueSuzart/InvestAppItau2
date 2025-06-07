using System.Collections.Generic;
using System.Threading.Tasks;
using InvestApp.Models;
using InvestApp.Services.DTOs;

namespace InvestApp.Data
{
    public interface IRepository
    {
        Task<IEnumerable<Operacao>> GetOperacoesPorUsuarioAsync(int usuarioId);
        Task<Cotacao?> GetUltimaCotacaoAsync(int ativoId);
        Task<Usuario?> GetUsuarioAsync(int usuarioId);
        Task<Ativo?> GetAtivoAsync(int ativoId);
        Task<IEnumerable<TotalInvestidoPorAtivo>> GetTotalInvestidoPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<PosicaoPorPapel>> GetPosicaoPorPapelAsync(int usuarioId);
        Task<PosicaoGlobal> GetPosicaoGlobalComPnlAsync(int usuarioId);
        Task<decimal> GetTotalCorretagemPorUsuarioAsync(int usuarioId);
        Task<Usuario> GetUsuarioPorEmailAsync(string email);
        IEnumerable<PosicaoParaAtualizacao> BuscarTodasPosicoes();
        PosicaoDTO? BuscarPosicao(int usuarioId, int ativoId);
        void AtualizarPosicao(PosicaoDTO posicao);
        Task<IEnumerable<Cotacao>> GetCotacoesPorAtivoAsync(int ativoId);
        Task<IEnumerable<Usuario>> ListarUsuariosAsync();
        void InsertPosicao(PosicaoDTO posicao);
        Task InserirCotacaoAsync(int ativoId, decimal precoUnitario, DateTime dataHora);
        Task UpdateOperacaoAsync(int id, OperacaoDTO dto);
        Task<Ativo?> GetAtivoPorCodigoAsync(string codigo);
        Task InserirOperacaoAsync(Operacao operacao);
    }

    public class PosicaoParaAtualizacao
    {
        public int UsuarioId { get; set; }
        public int AtivoId { get; set; }
        public string Ticker { get; set; } = string.Empty;
    }
} 