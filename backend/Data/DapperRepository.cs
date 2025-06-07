using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using InvestApp.Models;
using System.Data;
using Microsoft.Extensions.Configuration;
using InvestApp.Services.DTOs;

namespace InvestApp.Data
{
    public class DapperRepository : IRepository
    {
        private readonly string _connectionString;

        public DapperRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Server=localhost;Database=investdb;User Id=root;Password=Al101299*;";
        }

        public async Task<IEnumerable<Operacao>> GetOperacoesPorUsuarioAsync(int usuarioId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var operacoes = await connection.QueryAsync<Operacao>(
                @"SELECT 
                    o.id,
                    o.usuario_id AS UsuarioId,
                    o.ativo_id AS AtivoId,
                    a.codigo AS CodigoAtivo,
                    o.tipo_operacao AS TipoOperacao,
                    o.quantidade,
                    o.preco_unitario AS PrecoUnitario,
                    o.corretagem,
                    o.data_hora AS DataHora
                  FROM operacoes o
                  JOIN ativos a ON o.ativo_id = a.id
                  WHERE o.usuario_id = @UsuarioId",
                new { UsuarioId = usuarioId }
            );
            
            return operacoes;
        }

        public async Task<Cotacao?> GetUltimaCotacaoAsync(int ativoId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var cotacao = await connection.QueryFirstOrDefaultAsync<Cotacao>(
                @"SELECT * FROM cotacoes 
                  WHERE ativo_id = @AtivoId 
                  ORDER BY data_hora DESC 
                  LIMIT 1",
                new { AtivoId = ativoId }
            );
            
            return cotacao;
        }

        public async Task<Usuario?> GetUsuarioAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                "SELECT * FROM usuarios WHERE id = @Id",
                new { Id = id }
            );
            
            return usuario;
        }

        public async Task<Ativo?> GetAtivoAsync(int ativoId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var ativo = await connection.QueryFirstOrDefaultAsync<Ativo>(
                "SELECT * FROM ativos WHERE id = @AtivoId",
                new { AtivoId = ativoId }
            );
            
            return ativo;
        }

        public async Task<IEnumerable<TotalInvestidoPorAtivo>> GetTotalInvestidoPorUsuarioAsync(int usuarioId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var result = await connection.QueryAsync<TotalInvestidoPorAtivo>(
                @"SELECT a.id as AtivoId, a.codigo as Codigo, a.nome as Nome,
                         SUM(o.quantidade * o.preco_unitario) as TotalInvestido
                  FROM operacoes o
                  JOIN ativos a ON o.ativo_id = a.id
                  WHERE o.usuario_id = @UsuarioId
                  GROUP BY a.id, a.codigo, a.nome",
                new { UsuarioId = usuarioId }
            );
            
            return result ?? Enumerable.Empty<TotalInvestidoPorAtivo>();
        }

        public async Task<IEnumerable<PosicaoPorPapel>> GetPosicaoPorPapelAsync(int usuarioId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var result = await connection.QueryAsync<PosicaoPorPapel>(
                @"SELECT a.id as AtivoId, a.codigo as Codigo, a.nome as Nome, a.preco_atual as PrecoAtual,
                         SUM(CASE WHEN o.tipo_operacao = 'COMPRA' THEN o.quantidade ELSE -o.quantidade END) as QuantidadeLiquida,
                         SUM(CASE WHEN o.tipo_operacao = 'COMPRA' THEN o.quantidade * o.preco_unitario ELSE 0 END) /
                         NULLIF(SUM(CASE WHEN o.tipo_operacao = 'COMPRA' THEN o.quantidade ELSE 0 END), 0) as PrecoMedio
                  FROM operacoes o
                  JOIN ativos a ON o.ativo_id = a.id
                  WHERE o.usuario_id = @UsuarioId
                  GROUP BY a.id, a.codigo, a.nome, a.preco_atual
                  HAVING QuantidadeLiquida > 0",
                new { UsuarioId = usuarioId }
            );
            
            return result ?? Enumerable.Empty<PosicaoPorPapel>();
        }

        public async Task<PosicaoGlobal> GetPosicaoGlobalComPnlAsync(int usuarioId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var posicao = await connection.QueryFirstOrDefaultAsync<PosicaoGlobal>(
                @"SELECT 
                    SUM(CASE WHEN o.tipo_operacao = 'COMPRA' THEN o.quantidade * a.preco_atual ELSE -o.quantidade * a.preco_atual END) as ValorMercado,
                    SUM(CASE WHEN o.tipo_operacao = 'COMPRA' THEN o.quantidade * o.preco_unitario ELSE -o.quantidade * o.preco_unitario END) as CustoTotal,
                    SUM(CASE WHEN o.tipo_operacao = 'COMPRA' THEN o.quantidade * (a.preco_atual - o.preco_unitario) ELSE -o.quantidade * (a.preco_atual - o.preco_unitario) END) as PnL
                  FROM operacoes o
                  JOIN ativos a ON o.ativo_id = a.id
                  WHERE o.usuario_id = @UsuarioId",
                new { UsuarioId = usuarioId }
            );
            
            return posicao ?? new PosicaoGlobal { ValorMercado = 0, CustoTotal = 0, PnL = 0 };
        }

        public async Task<decimal> GetTotalCorretagemPorUsuarioAsync(int usuarioId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var total = await connection.ExecuteScalarAsync<decimal>(
                "SELECT SUM(corretagem) FROM operacoes WHERE usuario_id = @UsuarioId",
                new { UsuarioId = usuarioId }
            );
            
            return total;
        }

        public async Task<Usuario> GetUsuarioPorEmailAsync(string email)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(
                @"SELECT
                    id,
                    nome      AS Nome,
                    email     AS Email,
                    senha     AS Senha,
                    perc_corretagem AS PercCorretagem
                  FROM usuarios
                  WHERE email = @Email;",
                new { Email = email }
            );
        }

        public IEnumerable<PosicaoParaAtualizacao> BuscarTodasPosicoes()
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            return connection.Query<PosicaoParaAtualizacao>(
                @"SELECT usuario_id AS UsuarioId, ativo_id AS AtivoId, codigo as Ticker
                  FROM posicoes p
                  JOIN ativos a ON p.ativo_id = a.id"
            );
        }

        public PosicaoDTO? BuscarPosicao(int usuarioId, int ativoId)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var posicao = connection.QueryFirstOrDefault<PosicaoDTO>(
                @"SELECT 
                    p.usuario_id AS UsuarioId,
                    p.ativo_id AS AtivoId,
                    p.quantidade_liquida AS QuantidadeLiquida,
                    p.preco_medio AS PrecoMedio,
                    p.quantidade_liquida * a.preco_atual AS ValorMercado,
                    p.pnl AS PnL,
                    NOW() AS UltimaAtualizacao
                  FROM posicoes p
                  JOIN ativos a ON p.ativo_id = a.id
                  WHERE p.usuario_id = @UsuarioId AND p.ativo_id = @AtivoId",
                new { UsuarioId = usuarioId, AtivoId = ativoId }
            );

            return posicao;
        }

        public void AtualizarPosicao(PosicaoDTO posicao)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            connection.Execute(
                @"UPDATE posicoes
                  SET quantidade_liquida = @QuantidadeLiquida,
                      preco_medio = @PrecoMedio,
                      pnl = @PnL
                  WHERE usuario_id = @UsuarioId AND ativo_id = @AtivoId",
                posicao
            );
        }

        public async Task<IEnumerable<Cotacao>> GetCotacoesPorAtivoAsync(int ativoId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var cotacoes = await connection.QueryAsync<Cotacao>(
                "SELECT id AS Id, ativo_id AS AtivoId, preco_unitario AS PrecoUnitario, data_hora AS DataHora FROM cotacoes WHERE ativo_id = @AtivoId ORDER BY data_hora",
                new { AtivoId = ativoId }
            );
            return cotacoes;
        }

        public async Task<IEnumerable<Usuario>> ListarUsuariosAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            var usuarios = await connection.QueryAsync<Usuario>("SELECT id, nome FROM usuarios");
            return usuarios;
        }

        public void InsertPosicao(PosicaoDTO posicao)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            connection.Execute(
                @"INSERT INTO posicoes (usuario_id, ativo_id, quantidade_liquida, preco_medio, pnl, ultima_atualizacao)
                  VALUES (@UsuarioId, @AtivoId, @QuantidadeLiquida, @PrecoMedio, @PnL, @UltimaAtualizacao)",
                posicao
            );
        }

        public async Task InserirCotacaoAsync(int ativoId, decimal precoUnitario, DateTime dataHora)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                @"INSERT INTO cotacoes (ativo_id, preco_unitario, data_hora) VALUES (@AtivoId, @PrecoUnitario, @DataHora)",
                new { AtivoId = ativoId, PrecoUnitario = precoUnitario, DataHora = dataHora }
            );
        }

        public async Task UpdateOperacaoAsync(int id, OperacaoDTO dto)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                @"UPDATE operacoes
                  SET quantidade = @Quantidade,
                      preco_unitario = @Preco,
                      tipo_operacao = @TipoOperacao,
                      data_hora = @Data
                  WHERE id = @Id",
                new
                {
                    Id = id,
                    Quantidade = dto.Quantidade,
                    Preco = dto.Preco,
                    TipoOperacao = string.IsNullOrEmpty(dto.TipoOperacao) ? dto.Tipo : dto.TipoOperacao,
                    Data = dto.Data
                }
            );
        }

        public async Task<Ativo?> GetAtivoPorCodigoAsync(string codigo)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<Ativo>(
                "SELECT * FROM ativos WHERE codigo = @Codigo",
                new { Codigo = codigo }
            );
        }

        public async Task InserirOperacaoAsync(Operacao operacao)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(
                @"INSERT INTO operacoes (usuario_id, ativo_id, tipo_operacao, quantidade, preco_unitario, data_hora)
                  VALUES (@UsuarioId, @AtivoId, @TipoOperacao, @Quantidade, @PrecoUnitario, @DataHora)",
                operacao
            );
        }
    }
} 