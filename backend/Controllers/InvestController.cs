using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using InvestApp.Services;
using InvestApp.Services.DTOs;
using System.Collections.Generic;
using System;
using InvestApp.Models;

namespace InvestApp.Controllers;

[ApiController]
[Route("invest")]
public class InvestController : ControllerBase
{
    private readonly IInvestService _investService;

    public InvestController(IInvestService investService)
    {
        Console.WriteLine("[DEBUG] InvestController construído");
        _investService = investService;
    }

    [HttpGet("usuario/{usuarioId}/totalInvestido")]
    public async Task<IActionResult> GetTotalInvestidoPorUsuario(int usuarioId)
    {
        try
        {
            var lista = await _investService.GetTotalInvestidoPorUsuarioAsync(usuarioId);
            if (lista == null || !lista.Any())
                return NotFound($"Nenhum investimento encontrado para usuário {usuarioId}.");
            return Ok(lista);
        }
        catch (MySqlException ex)
        {
            return StatusCode(500, $"Erro de banco de dados: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpGet("usuario/{usuarioId}/posicaoPorPapel")]
    public async Task<IActionResult> GetPosicaoPorPapel(int usuarioId)
    {
        try
        {
            var lista = await _investService.GetPosicaoPorPapelAsync(usuarioId);
            if (lista == null || !lista.Any())
                return NotFound($"Nenhuma posição por papel para usuário {usuarioId}.");
            return Ok(lista);
        }
        catch (MySqlException ex)
        {
            return StatusCode(500, $"Erro de banco de dados: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpGet("usuario/{usuarioId}/posicaoGlobal")]
    public async Task<IActionResult> GetPosicaoGlobal(int usuarioId)
    {
        Console.WriteLine($"[InvestController] Iniciando busca de posição global para usuário {usuarioId}");
        try
        {
            var obj = await _investService.GetPosicaoGlobalComPnlAsync(usuarioId);
            Console.WriteLine($"[InvestController] Resultado da posição global: {System.Text.Json.JsonSerializer.Serialize(obj)}");
            if (obj == null || (obj.ValorMercado == 0 && obj.CustoTotal == 0 && obj.PnL == 0))
                return Ok(new { mensagem = "Não foi possível calcular a posição global. Cotação indisponível ou limite de requisições atingido." });
            return Ok(obj);
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"[InvestController] Erro de banco de dados: {ex.Message}");
            return Ok(new { mensagem = "Erro de banco de dados ao buscar posição global." });
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"[InvestController] Erro interno: {ex.Message}\n{ex.StackTrace}");
            return Ok(new { mensagem = "Erro interno ao buscar posição global." });
        }
    }

    [HttpGet("usuario/{usuarioId}/totalCorretagem")]
    public async Task<IActionResult> GetTotalCorretagem(int usuarioId)
    {
        try
        {
            var valor = await _investService.GetTotalCorretagemPorUsuarioAsync(usuarioId);
            return Ok(valor);
        }
        catch (MySqlException ex)
        {
            return StatusCode(500, $"Erro de banco de dados: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpPost("usuario/{usuarioId}/atualizarPosicoes")]
    public async Task<IActionResult> AtualizarPosicoes(int usuarioId)
    {
        try
        {
            // Atualiza todas as posições do usuário manualmente
            var posicoes = _investService.GetPosicaoPorPapelAsync(usuarioId);
            foreach (var posicao in await posicoes)
            {
                await HttpContext.RequestServices.GetRequiredService<IPosicaoService>()
                    .AtualizarPosicaoComCotacaoAsync(usuarioId, posicao.AtivoId, posicao.Codigo);
            }
            return Ok(new { mensagem = "Posições atualizadas com sucesso!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[InvestController] Erro ao atualizar posições manualmente: {ex.Message}");
            return Ok(new { mensagem = "Erro ao atualizar posições." });
        }
    }

    [HttpGet("usuario/{usuarioId}/historicoPortfolio")]
    public async Task<IActionResult> GetHistoricoPortfolio(int usuarioId)
    {
        Console.WriteLine($"[DEBUG] Iniciando GetHistoricoPortfolio para usuario {usuarioId}");
        try
        {
            // Busca todas as operações do usuário
            var operacoes = await _investService.GetOperacoesPorUsuarioAsync(usuarioId);
            Console.WriteLine($"[DEBUG] Operacoes carregadas: {(operacoes == null ? "null" : operacoes.Count().ToString())}");
            if (operacoes == null || !operacoes.Any())
                return Ok(new List<object>());

            // Busca todas as datas de operação e cotação
            var datasOperacoes = operacoes.Select(o => o.DataHora.Date).Distinct();
            var ativos = operacoes.Select(o => o.AtivoId).Distinct().ToList();
            Console.WriteLine($"[DEBUG] Ativos distintos: {string.Join(",", ativos)}");

            // Busca todas as datas de cotação dos ativos envolvidos
            var datasCotacoes = new List<System.DateTime>();
            foreach (var ativoId in ativos)
            {
                Console.WriteLine($"[DEBUG] Buscando cotações para ativo {ativoId}");
                var cotacoesAtivo = await _investService.GetCotacoesPorAtivoAsync(ativoId);
                Console.WriteLine($"[DEBUG] Qtde cotações encontradas para ativo {ativoId}: {(cotacoesAtivo == null ? "null" : cotacoesAtivo.Count().ToString())}");
                if (cotacoesAtivo != null)
                    datasCotacoes.AddRange(cotacoesAtivo.Select(c => c.DataHora.Date));
            }

            // Junta todas as datas relevantes (operações + cotações)
            var datas = datasOperacoes.Concat(datasCotacoes)
                .Where(d => d > DateTime.MinValue)
                .Distinct()
                .OrderBy(d => d)
                .ToList();
            Console.WriteLine($"[DEBUG] Total de datas relevantes: {datas.Count}");

            var historico = new List<object>();
            foreach (var data in datas)
            {
                decimal valorCarteira = 0;
                Console.WriteLine($"[DEBUG] Processando data: {data:yyyy-MM-dd}");
                foreach (var ativoId in ativos)
                {
                    var quantidade = operacoes
                        .Where(o => o.AtivoId == ativoId && o.DataHora.Date <= data)
                        .Sum(o => o.TipoOperacao == "COMPRA" ? o.Quantidade : -o.Quantidade);
                    Console.WriteLine($"[DEBUG] Ativo {ativoId} - Quantidade até {data:yyyy-MM-dd}: {quantidade}");

                    var cotacao = await _investService.GetUltimaCotacaoAteDataAsync(ativoId, data);
                    Console.WriteLine($"[DEBUG] Ativo {ativoId} - Cotação até {data:yyyy-MM-dd}: {(cotacao.HasValue ? cotacao.Value.ToString() : "null")}");
                    Console.WriteLine($"[DEBUG] Data: {data:yyyy-MM-dd}, Ativo: {ativoId}, Quantidade: {quantidade}, Cotação: {(cotacao.HasValue ? cotacao.Value.ToString() : "null")}");
                    if (cotacao.HasValue)
                        valorCarteira += quantidade * cotacao.Value;
                }
                historico.Add(new { data = data.ToString("yyyy-MM-dd"), valorCarteira });
            }
            Console.WriteLine($"[DEBUG] Histórico montado com {historico.Count} pontos");
            return Ok(historico);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"[InvestController] Erro ao buscar histórico do portfólio: {ex.Message}\n{ex.StackTrace}");
            return Ok(new List<object>());
        }
    }

    [HttpPost("cotacao/inserir")]
    public async Task<IActionResult> InserirCotacao([FromBody] InvestApp.Services.DTOs.CotacaoInsertDTO dto)
    {
        try
        {
            await _investService.InserirCotacaoAsync(dto.AtivoId, dto.PrecoUnitario, dto.DataHora);
            return Ok(new { mensagem = "Cotação inserida com sucesso!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao inserir cotação: {ex.Message}");
        }
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<IActionResult> GetUsuario(int usuarioId)
    {
        var usuario = await _investService.GetUsuarioAsync(usuarioId);
        if (usuario == null)
            return NotFound();
        return Ok(new { nome = usuario.Nome });
    }

    [HttpGet("ativo/{ativoId}/cotacoes")]
    public async Task<IActionResult> GetCotacoesPorAtivo(int ativoId)
    {
        try
        {
            var cotacoes = await _investService.GetCotacoesPorAtivoAsync(ativoId);
            if (cotacoes == null || !cotacoes.Any())
                return Ok(new List<object>());
            return Ok(cotacoes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[InvestController] Erro ao buscar cotações do ativo {ativoId}: {ex.Message}");
            return StatusCode(500, $"Erro ao buscar cotações: {ex.Message}");
        }
    }

    [HttpPut("operacao/{id}")]
    public async Task<IActionResult> EditarOperacao(int id, [FromBody] OperacaoDTO dto)
    {
        await _investService.UpdateOperacaoAsync(id, dto);
        return Ok(new { mensagem = "Operação atualizada com sucesso!" });
    }

    [HttpGet("usuario/{usuarioId}/operacoes")]
    public async Task<IActionResult> GetOperacoes(int usuarioId, int pagina = 1, int tamanho = 10, string ordenarPor = "data", string ordem = "desc", string? filtroAtivo = null)
    {
        var operacoes = await _investService.GetOperacoesPorUsuarioAsync(usuarioId);
        if (!string.IsNullOrEmpty(filtroAtivo))
        {
            operacoes = operacoes.Where(o => (o.CodigoAtivo ?? o.AtivoId.ToString()).ToLower().Contains(filtroAtivo.ToLower()));
        }
        operacoes = ordenarPor.ToLower() switch
        {
            "data" => ordem == "asc" ? operacoes.OrderBy(o => o.DataHora) : operacoes.OrderByDescending(o => o.DataHora),
            "preco" => ordem == "asc" ? operacoes.OrderBy(o => o.PrecoUnitario) : operacoes.OrderByDescending(o => o.PrecoUnitario),
            "valortotal" => ordem == "asc" ? operacoes.OrderBy(o => o.Quantidade * o.PrecoUnitario) : operacoes.OrderByDescending(o => o.Quantidade * o.PrecoUnitario),
            _ => operacoes.OrderByDescending(o => o.DataHora)
        };
        var total = operacoes.Count();
        operacoes = operacoes.Skip((pagina - 1) * tamanho).Take(tamanho);
        return Ok(new {
            operacoes = operacoes.Select(o => new {
                id = o.Id,
                data = o.DataHora,
                ativo = o.CodigoAtivo,
                tipo = o.TipoOperacao,
                quantidade = o.Quantidade,
                preco = o.PrecoUnitario,
                valorTotal = o.Quantidade * o.PrecoUnitario
            }),
            total
        });
    }

    [HttpPost("usuario/{usuarioId}/operacao")]
    public async Task<IActionResult> CriarOperacao(int usuarioId, [FromBody] OperacaoDTO dto)
    {
        var ativo = await _investService.GetAtivoPorCodigoAsync(dto.Ativo);
        if (ativo == null)
            return BadRequest("Ativo não encontrado.");
        var operacao = new Operacao
        {
            UsuarioId = usuarioId,
            AtivoId = ativo.Id,
            TipoOperacao = dto.TipoOperacao,
            Quantidade = dto.Quantidade,
            PrecoUnitario = dto.Preco,
            DataHora = dto.Data
        };
        await _investService.InserirOperacaoAsync(operacao);
        return Ok(new { mensagem = "Operação criada com sucesso!" });
    }
} 