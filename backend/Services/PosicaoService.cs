using InvestApp.Data;
using InvestApp.Services.DTOs;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Dapper;

namespace InvestApp.Services
{
    public interface IPosicaoService
    {
        void AtualizarPosicao(int usuarioId, int ativoId, decimal cotacaoAtual);
        Task AtualizarPosicaoComCotacaoAsync(int usuarioId, int ativoId, string ticker);
        Task AtualizarTodasPosicoesAsync();
    }

    public class PosicaoService : IPosicaoService
    {
        private readonly IRepository _repositorio;
        private readonly ICotacaoService _cotacaoService;
        private readonly string _connectionString;

        public PosicaoService(IRepository repositorio, ICotacaoService cotacaoService, string connectionString)
        {
            _repositorio = repositorio;
            _cotacaoService = cotacaoService;
            _connectionString = connectionString;
        }

        public void AtualizarPosicao(int usuarioId, int ativoId, decimal cotacaoAtual)
        {
            var posicao = _repositorio.BuscarPosicao(usuarioId, ativoId);
            if (posicao != null)
            {
                // Buscar o preco_atual do ativo
                var ativo = _repositorio.GetAtivoAsync(ativoId).Result;
                var precoAtual = cotacaoAtual;
                if (ativo != null && ativo.PrecoAtual > 0)
                {
                    precoAtual = ativo.PrecoAtual;
                }
                posicao.ValorMercado = posicao.QuantidadeLiquida * precoAtual;
                posicao.PnL = posicao.ValorMercado - (posicao.QuantidadeLiquida * posicao.PrecoMedio);
                posicao.UltimaAtualizacao = DateTime.Now;
                _repositorio.AtualizarPosicao(posicao);

                // Atualiza o preço atual do ativo
                using var connection = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
                connection.Open();
                connection.Execute(
                    @"UPDATE ativos 
                      SET preco_atual = @PrecoAtual 
                      WHERE id = @AtivoId",
                    new { PrecoAtual = precoAtual, AtivoId = ativoId }
                );
            }
        }

        public async Task AtualizarPosicaoComCotacaoAsync(int usuarioId, int ativoId, string ticker)
        {
            var cotacao = await _cotacaoService.ObterCotacaoAsync(ticker);
            if (!cotacao.HasValue || cotacao.Value == 0)
            {
                // Busca o preço atual do banco
                var ativo = await _repositorio.GetAtivoAsync(ativoId);
                if (ativo != null && ativo.PrecoAtual > 0)
                    cotacao = ativo.PrecoAtual;
            }
            if (cotacao.HasValue && cotacao.Value > 0)
            {
                AtualizarPosicao(usuarioId, ativoId, cotacao.Value);
            }
            else
            {
                Console.WriteLine($"[PosicaoService] Não foi possível obter cotação para o ticker {ticker} nem do banco. Nenhuma atualização realizada.");
            }
        }

        public async Task AtualizarTodasPosicoesAsync()
        {
            // Buscar todos os usuários
            var usuarios = await _repositorio.ListarUsuariosAsync();
            foreach (var usuario in usuarios)
            {
                // Buscar todos os ativos que o usuário possui operações
                var operacoes = await _repositorio.GetOperacoesPorUsuarioAsync(usuario.Id);
                var ativosIds = operacoes.Select(o => o.AtivoId).Distinct();
                foreach (var ativoId in ativosIds)
                {
                    // Se não existe posição, criar
                    var posicao = _repositorio.BuscarPosicao(usuario.Id, ativoId);
                    if (posicao == null)
                    {
                        // Calcular quantidade líquida e preço médio
                        var compras = operacoes.Where(o => o.AtivoId == ativoId && o.TipoOperacao == "COMPRA").ToList();
                        var vendas = operacoes.Where(o => o.AtivoId == ativoId && o.TipoOperacao == "VENDA").ToList();
                        var quantidadeLiquida = compras.Sum(o => o.Quantidade) - vendas.Sum(o => o.Quantidade);
                        var precoMedio = compras.Sum(o => o.Quantidade * o.PrecoUnitario) / (compras.Sum(o => o.Quantidade) == 0 ? 1 : compras.Sum(o => o.Quantidade));
                        var novaPosicao = new InvestApp.Services.DTOs.PosicaoDTO
                        {
                            UsuarioId = usuario.Id,
                            AtivoId = ativoId,
                            QuantidadeLiquida = (int)quantidadeLiquida,
                            PrecoMedio = precoMedio,
                            ValorMercado = 0,
                            PnL = 0,
                            UltimaAtualizacao = DateTime.Now
                        };
                        _repositorio.InsertPosicao(novaPosicao);
                        // Atualizar imediatamente a posição criada
                        var ticker = operacoes.FirstOrDefault(o => o.AtivoId == ativoId)?.CodigoAtivo;
                        if (!string.IsNullOrEmpty(ticker))
                        {
                            await AtualizarPosicaoComCotacaoAsync(usuario.Id, ativoId, ticker);
                        }
                    }
                }
            }
            // Agora atualizar todas as posições normalmente
            var posicoes = _repositorio.BuscarTodasPosicoes();
            foreach (var posicao in posicoes)
            {
                if (!string.IsNullOrEmpty(posicao.Ticker))
                {
                    await AtualizarPosicaoComCotacaoAsync(posicao.UsuarioId, posicao.AtivoId, posicao.Ticker);
                }
            }
        }
    }
} 