using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestApp.Data;
using InvestApp.Models;
using InvestApp.Services.DTOs;
using MySql.Data.MySqlClient;
using Dapper;

namespace InvestApp.Services
{
    public class InvestService : IInvestService
    {
        private readonly IRepository _repository;
        private readonly ICotacaoService _cotacaoService;

        public InvestService(IRepository repository, ICotacaoService cotacaoService)
        {
            Console.WriteLine($"[DEBUG] InvestService construído. repository nulo? {repository == null}");
            _repository = repository;
            _cotacaoService = cotacaoService;
        }

        public async Task<Usuario?> GetUsuarioAsync(int id)
        {
            return await _repository.GetUsuarioAsync(id);
        }

        public async Task<IEnumerable<TotalInvestidoPorAtivoDto>> GetTotalInvestidoPorUsuarioAsync(int usuarioId)
        {
            var totais = await _repository.GetTotalInvestidoPorUsuarioAsync(usuarioId);
            return totais.Select(t => new TotalInvestidoPorAtivoDto
            {
                AtivoId = t.AtivoId,
                Codigo = t.Codigo,
                TotalInvestido = t.TotalInvestido
            });
        }

        public async Task<IEnumerable<PosicaoPorPapelDto>> GetPosicaoPorPapelAsync(int usuarioId)
        {
            var posicoes = await _repository.GetPosicaoPorPapelAsync(usuarioId);
            return posicoes.Select(p => new PosicaoPorPapelDto
            {
                AtivoId = p.AtivoId,
                Codigo = p.Codigo,
                QuantidadeLiquida = p.QuantidadeLiquida,
                PrecoMedio = p.PrecoMedio,
                PrecoAtual = p.PrecoAtual
            });
        }

        public async Task<GlobalPositionDto> GetPosicaoGlobalComPnlAsync(int usuarioId)
        {
            var posicoes = await GetPosicaoPorPapelAsync(usuarioId);
            var posicaoGlobal = new GlobalPositionDto { UsuarioId = usuarioId };

            foreach (var posicao in posicoes.Where(p => p.QuantidadeLiquida > 0))
            {
                var precoAtual = await _cotacaoService.ObterCotacaoAsync(posicao.Codigo) ?? 0m;
                Console.WriteLine($"[DEBUG] Cotação API para {posicao.Codigo}: {precoAtual}");
                
                var valorMercado = posicao.QuantidadeLiquida * precoAtual;
                var custoTotal = posicao.QuantidadeLiquida * posicao.PrecoMedio;

                posicaoGlobal.ValorMercado += valorMercado;
                posicaoGlobal.CustoTotal += custoTotal;
            }

            posicaoGlobal.PnL = posicaoGlobal.ValorMercado - posicaoGlobal.CustoTotal;

            // Preencher operações
            var operacoes = await _repository.GetOperacoesPorUsuarioAsync(usuarioId);
            posicaoGlobal.Operacoes = operacoes.Select(o => new OperacaoDTO {
                Id = o.Id,
                Data = o.DataHora,
                Ativo = o.CodigoAtivo,
                Tipo = o.TipoOperacao,
                Quantidade = (int)o.Quantidade,
                Preco = o.PrecoUnitario
            }).ToList();

            // Preencher histórico da carteira (real)
            var historico = new List<HistoricoCarteiraDTO>();
            var operacoesUsuario = operacoes.ToList();
            var ativosIds = operacoesUsuario.Select(o => o.AtivoId).Distinct().ToList();
            var datas = operacoesUsuario.Select(o => o.DataHora.Date).Distinct().OrderBy(d => d).ToList();
            foreach (var data in datas)
            {
                decimal valorCarteira = 0;
                foreach (var ativoId in ativosIds)
                {
                    var quantidade = operacoesUsuario
                        .Where(o => o.AtivoId == ativoId && o.DataHora.Date <= data)
                        .Sum(o => o.TipoOperacao == "COMPRA" ? o.Quantidade : -o.Quantidade);
                    var cotacao = await GetUltimaCotacaoAteDataAsync(ativoId, data) ?? 0m;
                    valorCarteira += quantidade * cotacao;
                }
                historico.Add(new HistoricoCarteiraDTO {
                    Data = data.ToString("yyyy-MM-dd"),
                    Valor = valorCarteira
                });
            }
            posicaoGlobal.HistoricoCarteira = historico;

            // Adiciona ponto para o dia de hoje com as cotações mais atuais
            var hoje = DateTime.Today;
            if (!historico.Any(h => h.Data == hoje.ToString("yyyy-MM-dd")))
            {
                decimal valorCarteiraAtual = 0;
                foreach (var posicao in posicoes.Where(p => p.QuantidadeLiquida > 0))
                {
                    var precoAtual = await _cotacaoService.ObterCotacaoAsync(posicao.Codigo) ?? 0m;
                    valorCarteiraAtual += posicao.QuantidadeLiquida * precoAtual;
                }
                historico.Add(new HistoricoCarteiraDTO {
                    Data = hoje.ToString("yyyy-MM-dd"),
                    Valor = valorCarteiraAtual
                });
            }

            // Preencher distribuição de ativos (total investido por ativo)
            var distribuicao = new List<DistribuicaoAtivoDTO>();
            var codigosAtivos = operacoesUsuario.Select(o => o.CodigoAtivo).Distinct();
            foreach (var codigo in codigosAtivos)
            {
                var totalInvestido = operacoesUsuario
                    .Where(o => o.CodigoAtivo == codigo && o.TipoOperacao == "COMPRA")
                    .Sum(o => o.Quantidade * o.PrecoUnitario);
                distribuicao.Add(new DistribuicaoAtivoDTO
                {
                    Ativo = codigo,
                    Valor = totalInvestido
                });
            }
            posicaoGlobal.DistribuicaoAtivos = distribuicao;

            return posicaoGlobal;
        }

        public async Task<decimal> GetTotalCorretagemPorUsuarioAsync(int usuarioId)
        {
            var operacoes = await _repository.GetOperacoesPorUsuarioAsync(usuarioId);
            return operacoes.Sum(o => o.Corretagem);
        }

        public async Task<IEnumerable<Operacao>> GetOperacoesPorUsuarioAsync(int usuarioId)
        {
            return await _repository.GetOperacoesPorUsuarioAsync(usuarioId);
        }

        public async Task<decimal?> GetUltimaCotacaoAteDataAsync(int ativoId, DateTime data)
        {
            var cotacoes = await _repository.GetCotacoesPorAtivoAsync(ativoId);
            foreach (var c in cotacoes)
            {
                Console.WriteLine($"[DEBUG] (Service) Cotação carregada: Ativo {c.AtivoId}, Preço {c.PrecoUnitario}, Data {c.DataHora}");
            }
            var cotacao = cotacoes
                .Where(c => c.DataHora <= data)
                .OrderByDescending(c => c.DataHora)
                .FirstOrDefault();
            Console.WriteLine($"[DEBUG] (Service) Ativo: {ativoId}, Data: {data}, Cotação encontrada: {(cotacao != null ? cotacao.PrecoUnitario.ToString() : "null")}, Data cotação: {(cotacao != null ? cotacao.DataHora.ToString() : "null")}");
            if (cotacao == null || cotacao.PrecoUnitario == 0)
                return null;
            return cotacao.PrecoUnitario;
        }

        public async Task<IEnumerable<Cotacao>> GetCotacoesPorAtivoAsync(int ativoId)
        {
            var cotacoes = (await _repository.GetCotacoesPorAtivoAsync(ativoId)).ToList();
            var hoje = DateTime.Today;
            // Se não existe cotação para hoje, busca da API externa
            if (!cotacoes.Any(c => c.DataHora.Date == hoje))
            {
                var ativo = await _repository.GetAtivoAsync(ativoId);
                if (ativo != null && !string.IsNullOrEmpty(ativo.Codigo))
                {
                    var preco = await _cotacaoService.ObterCotacaoAsync(ativo.Codigo);
                    if (preco.HasValue && preco.Value > 0)
                    {
                        await InserirCotacaoAsync(ativoId, preco.Value, DateTime.Now);
                        // Atualiza a lista de cotações
                        cotacoes = (await _repository.GetCotacoesPorAtivoAsync(ativoId)).ToList();
                    }
                }
            }
            return cotacoes;
        }

        private decimal CalcularPrecoMedio(List<Operacao> compras)
        {
            if (!compras.Any()) return 0m;
            
            var somaValor = compras.Sum(o => o.Quantidade * o.PrecoUnitario);
            var somaQtd = compras.Sum(o => o.Quantidade);
            
            return somaQtd > 0 ? somaValor / somaQtd : 0m;
        }

        public async Task InserirCotacaoAsync(int ativoId, decimal precoUnitario, DateTime dataHora)
        {
            await _repository.InserirCotacaoAsync(ativoId, precoUnitario, dataHora);
        }

        public async Task UpdateOperacaoAsync(int id, OperacaoDTO dto)
        {
            await _repository.UpdateOperacaoAsync(id, dto);
        }

        public async Task<Ativo?> GetAtivoPorCodigoAsync(string codigo)
        {
            return await _repository.GetAtivoPorCodigoAsync(codigo);
        }

        public async Task InserirOperacaoAsync(Operacao operacao)
        {
            await _repository.InserirOperacaoAsync(operacao);
        }
    }
} 