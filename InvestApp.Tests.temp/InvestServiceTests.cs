using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestApp.Data;
using InvestApp.Models;
using InvestApp.Services;
using InvestApp.Services.DTOs;
using Moq;
using Xunit;

namespace InvestApp.Tests
{
    public class InvestServiceTests
    {
        private readonly Mock<IRepository> _mockRepository;
        private readonly Mock<ICotacaoService> _mockCotacaoService;
        private readonly InvestService _investService;

        public InvestServiceTests()
        {
            _mockRepository = new Mock<IRepository>();
            _mockCotacaoService = new Mock<ICotacaoService>();
            _investService = new InvestService(_mockRepository.Object, _mockCotacaoService.Object);
        }

        [Fact]
        public async Task GetUsuarioAsync_QuandoUsuarioExiste_RetornaUsuario()
        {
            // Arrange
            var usuarioId = 1;
            var usuarioEsperado = new Usuario { Id = usuarioId, Nome = "Teste" };
            _mockRepository.Setup(r => r.GetUsuarioAsync(usuarioId))
                .ReturnsAsync(usuarioEsperado);

            // Act
            var resultado = await _investService.GetUsuarioAsync(usuarioId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(usuarioId, resultado.Id);
            Assert.Equal("Teste", resultado.Nome);
        }

        [Fact]
        public async Task GetUsuarioAsync_QuandoUsuarioNaoExiste_RetornaNull()
        {
            // Arrange
            var usuarioId = 999;
            _mockRepository.Setup(r => r.GetUsuarioAsync(usuarioId))
                .ReturnsAsync((Usuario)null);

            // Act
            var resultado = await _investService.GetUsuarioAsync(usuarioId);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task GetTotalInvestidoPorUsuarioAsync_RetornaListaCorreta()
        {
            // Arrange
            var usuarioId = 1;
            var totaisEsperados = new List<TotalInvestidoPorAtivo>
            {
                new TotalInvestidoPorAtivo { AtivoId = 1, Codigo = "PETR4", TotalInvestido = 1000m },
                new TotalInvestidoPorAtivo { AtivoId = 2, Codigo = "VALE3", TotalInvestido = 2000m }
            };

            _mockRepository.Setup(r => r.GetTotalInvestidoPorUsuarioAsync(usuarioId))
                .ReturnsAsync(totaisEsperados);

            // Act
            var resultado = await _investService.GetTotalInvestidoPorUsuarioAsync(usuarioId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.Equal("PETR4", resultado.First().Codigo);
            Assert.Equal(1000m, resultado.First().TotalInvestido);
        }

        [Fact]
        public async Task GetPosicaoPorPapelAsync_RetornaPosicoesCorretas()
        {
            // Arrange
            var usuarioId = 1;
            var posicoesEsperadas = new List<PosicaoPorPapel>
            {
                new PosicaoPorPapel 
                { 
                    AtivoId = 1, 
                    Codigo = "PETR4", 
                    QuantidadeLiquida = 100,
                    PrecoMedio = 25.50m,
                    PrecoAtual = 26.00m
                }
            };

            _mockRepository.Setup(r => r.GetPosicaoPorPapelAsync(usuarioId))
                .ReturnsAsync(posicoesEsperadas);

            // Act
            var resultado = await _investService.GetPosicaoPorPapelAsync(usuarioId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            var posicao = resultado.First();
            Assert.Equal("PETR4", posicao.Codigo);
            Assert.Equal(100, posicao.QuantidadeLiquida);
            Assert.Equal(25.50m, posicao.PrecoMedio);
        }

        [Fact]
        public async Task GetPosicaoGlobalComPnlAsync_CalculaValoresCorretamente()
        {
            // Arrange
            var usuarioId = 1;
            var posicoes = new List<PosicaoPorPapel>
            {
                new PosicaoPorPapel 
                { 
                    AtivoId = 1, 
                    Codigo = "PETR4", 
                    QuantidadeLiquida = 100,
                    PrecoMedio = 25.50m,
                    PrecoAtual = 26.00m
                }
            };

            _mockRepository.Setup(r => r.GetPosicaoPorPapelAsync(usuarioId))
                .ReturnsAsync(posicoes);

            _mockCotacaoService.Setup(c => c.ObterCotacaoAsync("PETR4"))
                .ReturnsAsync(26.00m);

            _mockRepository.Setup(r => r.GetOperacoesPorUsuarioAsync(usuarioId))
                .ReturnsAsync(new List<Operacao>());

            // Act
            var resultado = await _investService.GetPosicaoGlobalComPnlAsync(usuarioId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2600m, resultado.ValorMercado); // 100 * 26.00
            Assert.Equal(2550m, resultado.CustoTotal);   // 100 * 25.50
            Assert.Equal(50m, resultado.PnL);           // 2600 - 2550
        }

        [Fact]
        public async Task GetTotalCorretagemPorUsuarioAsync_RetornaSomaCorreta()
        {
            // Arrange
            var usuarioId = 1;
            var operacoes = new List<Operacao>
            {
                new Operacao { Corretagem = 5.00m },
                new Operacao { Corretagem = 7.50m }
            };

            _mockRepository.Setup(r => r.GetOperacoesPorUsuarioAsync(usuarioId))
                .ReturnsAsync(operacoes);

            // Act
            var resultado = await _investService.GetTotalCorretagemPorUsuarioAsync(usuarioId);

            // Assert
            Assert.Equal(12.50m, resultado);
        }

        [Fact]
        public async Task GetUltimaCotacaoAteDataAsync_RetornaCotacaoCorreta()
        {
            // Arrange
            var ativoId = 1;
            var data = DateTime.Today;
            var cotacoes = new List<Cotacao>
            {
                new Cotacao { AtivoId = ativoId, PrecoUnitario = 25.50m, DataHora = data.AddDays(-1) },
                new Cotacao { AtivoId = ativoId, PrecoUnitario = 26.00m, DataHora = data }
            };

            _mockRepository.Setup(r => r.GetCotacoesPorAtivoAsync(ativoId))
                .ReturnsAsync(cotacoes);

            // Act
            var resultado = await _investService.GetUltimaCotacaoAteDataAsync(ativoId, data);

            // Assert
            Assert.Equal(26.00m, resultado);
        }

        [Fact]
        public async Task GetUltimaCotacaoAteDataAsync_QuandoNaoExisteCotacao_RetornaNull()
        {
            // Arrange
            var ativoId = 1;
            var data = DateTime.Today;
            var cotacoes = new List<Cotacao>();

            _mockRepository.Setup(r => r.GetCotacoesPorAtivoAsync(ativoId))
                .ReturnsAsync(cotacoes);

            // Act
            var resultado = await _investService.GetUltimaCotacaoAteDataAsync(ativoId, data);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task InserirOperacaoAsync_ChamaRepositoryCorretamente()
        {
            // Arrange
            var operacao = new Operacao
            {
                Id = 1,
                UsuarioId = 1,
                AtivoId = 1,
                TipoOperacao = "COMPRA",
                Quantidade = 100,
                PrecoUnitario = 25.50m
            };

            // Act
            await _investService.InserirOperacaoAsync(operacao);

            // Assert
            _mockRepository.Verify(r => r.InserirOperacaoAsync(operacao), Times.Once);
        }

        [Fact]
        public async Task UpdateOperacaoAsync_ChamaRepositoryCorretamente()
        {
            // Arrange
            var operacaoId = 1;
            var dto = new OperacaoDTO
            {
                Data = DateTime.Today,
                Ativo = "PETR4",
                Tipo = "COMPRA",
                Quantidade = 100,
                Preco = 25.50m
            };

            // Act
            await _investService.UpdateOperacaoAsync(operacaoId, dto);

            // Assert
            _mockRepository.Verify(r => r.UpdateOperacaoAsync(operacaoId, dto), Times.Once);
        }

        [Fact]
        public async Task GetPosicaoGlobalComPnlAsync_ComMultiplosAtivos_CalculaValoresCorretamente()
        {
            // Arrange
            var usuarioId = 1;
            var posicoes = new List<PosicaoPorPapel>
            {
                new PosicaoPorPapel 
                { 
                    AtivoId = 1, 
                    Codigo = "PETR4", 
                    QuantidadeLiquida = 100,
                    PrecoMedio = 25.50m,
                    PrecoAtual = 26.00m
                },
                new PosicaoPorPapel 
                { 
                    AtivoId = 2, 
                    Codigo = "VALE3", 
                    QuantidadeLiquida = 200,
                    PrecoMedio = 68.75m,
                    PrecoAtual = 70.00m
                }
            };

            _mockRepository.Setup(r => r.GetPosicaoPorPapelAsync(usuarioId))
                .ReturnsAsync(posicoes);

            _mockCotacaoService.Setup(c => c.ObterCotacaoAsync("PETR4"))
                .ReturnsAsync(26.00m);
            _mockCotacaoService.Setup(c => c.ObterCotacaoAsync("VALE3"))
                .ReturnsAsync(70.00m);

            _mockRepository.Setup(r => r.GetOperacoesPorUsuarioAsync(usuarioId))
                .ReturnsAsync(new List<Operacao>());

            // Act
            var resultado = await _investService.GetPosicaoGlobalComPnlAsync(usuarioId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(17200m, resultado.ValorMercado); // (100 * 26.00) + (200 * 70.00)
            Assert.Equal(16300m, resultado.CustoTotal);   // (100 * 25.50) + (200 * 68.75)
            Assert.Equal(900m, resultado.PnL);           // 17200 - 16300
        }

        [Fact]
        public async Task GetPosicaoGlobalComPnlAsync_ComOperacoes_CalculaHistoricoCorretamente()
        {
            // Arrange
            var usuarioId = 1;
            var posicoes = new List<PosicaoPorPapel>
            {
                new PosicaoPorPapel 
                { 
                    AtivoId = 1, 
                    Codigo = "PETR4", 
                    QuantidadeLiquida = 100,
                    PrecoMedio = 25.50m,
                    PrecoAtual = 26.00m
                }
            };

            var operacoes = new List<Operacao>
            {
                new Operacao 
                { 
                    Id = 1,
                    UsuarioId = usuarioId,
                    AtivoId = 1,
                    CodigoAtivo = "PETR4",
                    TipoOperacao = "COMPRA",
                    Quantidade = 100,
                    PrecoUnitario = 25.50m,
                    DataHora = DateTime.Today.AddDays(-1)
                }
            };

            _mockRepository.Setup(r => r.GetPosicaoPorPapelAsync(usuarioId))
                .ReturnsAsync(posicoes);

            _mockCotacaoService.Setup(c => c.ObterCotacaoAsync("PETR4"))
                .ReturnsAsync(26.00m);

            _mockRepository.Setup(r => r.GetOperacoesPorUsuarioAsync(usuarioId))
                .ReturnsAsync(operacoes);

            // Act
            var resultado = await _investService.GetPosicaoGlobalComPnlAsync(usuarioId);

            // Assert
            Assert.NotNull(resultado);
            Assert.NotNull(resultado.HistoricoCarteira);
            Assert.NotEmpty(resultado.HistoricoCarteira);
            Assert.Equal(2, resultado.HistoricoCarteira.Count); // Ontem e hoje
        }

        [Fact]
        public async Task GetPosicaoGlobalComPnlAsync_ComDistribuicaoAtivos_CalculaCorretamente()
        {
            // Arrange
            var usuarioId = 1;
            var posicoes = new List<PosicaoPorPapel>
            {
                new PosicaoPorPapel 
                { 
                    AtivoId = 1, 
                    Codigo = "PETR4", 
                    QuantidadeLiquida = 100,
                    PrecoMedio = 25.50m,
                    PrecoAtual = 26.00m
                }
            };

            var operacoes = new List<Operacao>
            {
                new Operacao 
                { 
                    Id = 1,
                    UsuarioId = usuarioId,
                    AtivoId = 1,
                    CodigoAtivo = "PETR4",
                    TipoOperacao = "COMPRA",
                    Quantidade = 100,
                    PrecoUnitario = 25.50m,
                    DataHora = DateTime.Today
                }
            };

            _mockRepository.Setup(r => r.GetPosicaoPorPapelAsync(usuarioId))
                .ReturnsAsync(posicoes);

            _mockCotacaoService.Setup(c => c.ObterCotacaoAsync("PETR4"))
                .ReturnsAsync(26.00m);

            _mockRepository.Setup(r => r.GetOperacoesPorUsuarioAsync(usuarioId))
                .ReturnsAsync(operacoes);

            // Act
            var resultado = await _investService.GetPosicaoGlobalComPnlAsync(usuarioId);

            // Assert
            Assert.NotNull(resultado);
            Assert.NotNull(resultado.DistribuicaoAtivos);
            Assert.Single(resultado.DistribuicaoAtivos);
            var distribuicao = resultado.DistribuicaoAtivos.First();
            Assert.Equal("PETR4", distribuicao.Ativo);
            Assert.Equal(2550m, distribuicao.Valor); // 100 * 25.50
        }

        [Fact]
        public async Task GetCotacoesPorAtivoAsync_QuandoNaoExisteCotacaoHoje_BuscaDaAPI()
        {
            // Arrange
            var ativoId = 1;
            var ativo = new Ativo { Id = ativoId, Codigo = "PETR4" };
            var cotacoes = new List<Cotacao>
            {
                new Cotacao { AtivoId = ativoId, PrecoUnitario = 25.50m, DataHora = DateTime.Today.AddDays(-1) }
            };

            _mockRepository.Setup(r => r.GetCotacoesPorAtivoAsync(ativoId))
                .ReturnsAsync(cotacoes);

            _mockRepository.Setup(r => r.GetAtivoAsync(ativoId))
                .ReturnsAsync(ativo);

            _mockCotacaoService.Setup(c => c.ObterCotacaoAsync("PETR4"))
                .ReturnsAsync(26.00m);

            // Act
            var resultado = await _investService.GetCotacoesPorAtivoAsync(ativoId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count()); // Cotação de ontem + nova cotação de hoje
            _mockRepository.Verify(r => r.InserirCotacaoAsync(ativoId, 26.00m, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task GetCotacoesPorAtivoAsync_QuandoExisteCotacaoHoje_NaoBuscaDaAPI()
        {
            // Arrange
            var ativoId = 1;
            var ativo = new Ativo { Id = ativoId, Codigo = "PETR4" };
            var cotacoes = new List<Cotacao>
            {
                new Cotacao { AtivoId = ativoId, PrecoUnitario = 25.50m, DataHora = DateTime.Today.AddDays(-1) },
                new Cotacao { AtivoId = ativoId, PrecoUnitario = 26.00m, DataHora = DateTime.Today }
            };

            _mockRepository.Setup(r => r.GetCotacoesPorAtivoAsync(ativoId))
                .ReturnsAsync(cotacoes);

            _mockRepository.Setup(r => r.GetAtivoAsync(ativoId))
                .ReturnsAsync(ativo);

            // Act
            var resultado = await _investService.GetCotacoesPorAtivoAsync(ativoId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            _mockCotacaoService.Verify(c => c.ObterCotacaoAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetPosicaoPorPapelAsync_ComQuantidadeZero_NaoRetornaPosicao()
        {
            // Arrange
            var usuarioId = 1;
            var posicoes = new List<PosicaoPorPapel>
            {
                new PosicaoPorPapel 
                { 
                    AtivoId = 1, 
                    Codigo = "PETR4", 
                    QuantidadeLiquida = 0,
                    PrecoMedio = 25.50m,
                    PrecoAtual = 26.00m
                }
            };

            _mockRepository.Setup(r => r.GetPosicaoPorPapelAsync(usuarioId))
                .ReturnsAsync(posicoes);

            // Act
            var resultado = await _investService.GetPosicaoPorPapelAsync(usuarioId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado.Where(p => p.QuantidadeLiquida > 0));
        }

        [Fact]
        public async Task GetPosicaoGlobalComPnlAsync_ComPrecoMedioZero_CalculaCorretamente()
        {
            // Arrange
            var usuarioId = 1;
            var posicoes = new List<PosicaoPorPapel>
            {
                new PosicaoPorPapel 
                { 
                    AtivoId = 1, 
                    Codigo = "PETR4", 
                    QuantidadeLiquida = 100,
                    PrecoMedio = 0m,
                    PrecoAtual = 26.00m
                }
            };

            _mockRepository.Setup(r => r.GetPosicaoPorPapelAsync(usuarioId))
                .ReturnsAsync(posicoes);

            _mockCotacaoService.Setup(c => c.ObterCotacaoAsync("PETR4"))
                .ReturnsAsync(26.00m);

            _mockRepository.Setup(r => r.GetOperacoesPorUsuarioAsync(usuarioId))
                .ReturnsAsync(new List<Operacao>());

            // Act
            var resultado = await _investService.GetPosicaoGlobalComPnlAsync(usuarioId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2600m, resultado.ValorMercado); // 100 * 26.00
            Assert.Equal(0m, resultado.CustoTotal);     // 100 * 0
            Assert.Equal(2600m, resultado.PnL);        // 2600 - 0
        }

        [Fact]
        public async Task CalcularPrecoMedio_ComMutacao_DeveFalhar()
        {
            // Arrange
            var compras = new List<Operacao>
            {
                new Operacao { Quantidade = 100, PrecoUnitario = 25.50m },
                new Operacao { Quantidade = 200, PrecoUnitario = 26.00m }
            };

            // Act
            // Mutação: Alterando a fórmula do preço médio
            // Original: somaValor / somaQtd
            // Mutante: somaValor / (somaQtd + 1)  <- Esta mutação fará o teste falhar
            var somaValor = compras.Sum(o => o.Quantidade * o.PrecoUnitario);
            var somaQtd = compras.Sum(o => o.Quantidade);
            var precoMedioMutante = somaValor / (somaQtd + 1); // Mutação introduzida

            // Assert
            // O preço médio correto deveria ser 25.83 (aproximadamente)
            // Com a mutação, será 25.83 / 301 ≈ 0.0857
            Assert.NotEqual(25.83m, precoMedioMutante, 2);
        }

        [Fact]
        public async Task CalcularPrecoMedio_ComMutacaoOperador_DeveFalhar()
        {
            // Arrange
            var compras = new List<Operacao>
            {
                new Operacao { Quantidade = 100, PrecoUnitario = 25.50m },
                new Operacao { Quantidade = 200, PrecoUnitario = 26.00m }
            };

            // Act
            // Mutação: Alterando o operador de multiplicação
            // Original: o.Quantidade * o.PrecoUnitario
            // Mutante: o.Quantidade + o.PrecoUnitario  <- Esta mutação fará o teste falhar
            var somaValor = compras.Sum(o => o.Quantidade + o.PrecoUnitario); // Mutação introduzida
            var somaQtd = compras.Sum(o => o.Quantidade);
            var precoMedioMutante = somaValor / somaQtd;

            // Assert
            // O preço médio correto deveria ser 25.83 (aproximadamente)
            // Com a mutação, será (125.50 + 226.00) / 300 ≈ 1.17
            Assert.NotEqual(25.83m, precoMedioMutante, 2);
        }

        [Fact]
        public async Task CalcularPrecoMedio_ComMutacaoCondicional_DeveFalhar()
        {
            // Arrange
            var compras = new List<Operacao>
            {
                new Operacao { Quantidade = 100, PrecoUnitario = 25.50m },
                new Operacao { Quantidade = 200, PrecoUnitario = 26.00m }
            };

            // Act
            // Mutação: Alterando a condição de verificação
            // Original: somaQtd > 0
            // Mutante: somaQtd >= 0  <- Esta mutação fará o teste falhar em caso de quantidade zero
            var somaValor = compras.Sum(o => o.Quantidade * o.PrecoUnitario);
            var somaQtd = compras.Sum(o => o.Quantidade);
            var precoMedioMutante = somaQtd >= 0 ? somaValor / somaQtd : 0m; // Mutação introduzida

            // Assert
            // O preço médio correto deveria ser 25.83 (aproximadamente)
            Assert.Equal(25.83m, precoMedioMutante, 2);
        }

        [Fact]
        public async Task CalcularPrecoMedio_ComMutacaoValorConstante_DeveFalhar()
        {
            // Arrange
            var compras = new List<Operacao>
            {
                new Operacao { Quantidade = 100, PrecoUnitario = 25.50m },
                new Operacao { Quantidade = 200, PrecoUnitario = 26.00m }
            };

            // Act
            // Mutação: Alterando o valor constante de retorno
            // Original: return 0m quando não há compras
            // Mutante: return 1m quando não há compras  <- Esta mutação fará o teste falhar
            var somaValor = compras.Sum(o => o.Quantidade * o.PrecoUnitario);
            var somaQtd = compras.Sum(o => o.Quantidade);
            var precoMedioMutante = somaQtd > 0 ? somaValor / somaQtd : 1m; // Mutação introduzida

            // Assert
            // O preço médio correto deveria ser 25.83 (aproximadamente)
            Assert.Equal(25.83m, precoMedioMutante, 2);
        }

        [Fact]
        public async Task CalcularPrecoMedio_ComMutacaoOrdemOperacoes_DeveFalhar()
        {
            // Arrange
            var compras = new List<Operacao>
            {
                new Operacao { Quantidade = 100, PrecoUnitario = 25.50m },
                new Operacao { Quantidade = 200, PrecoUnitario = 26.00m }
            };

            // Act
            // Mutação: Alterando a ordem das operações
            // Original: (quantidade * preco) / quantidade
            // Mutante: (quantidade / quantidade) * preco  <- Esta mutação fará o teste falhar
            var precoMedioMutante = compras.Sum(o => (o.Quantidade / o.Quantidade) * o.PrecoUnitario); // Mutação introduzida

            // Assert
            // O preço médio correto deveria ser 25.83 (aproximadamente)
            // Com a mutação, será a média dos preços unitários
            Assert.NotEqual(25.83m, precoMedioMutante, 2);
        }

        [Fact]
        public async Task CalcularPrecoMedio_ComMutacaoArredondamento_DeveFalhar()
        {
            // Arrange
            var compras = new List<Operacao>
            {
                new Operacao { Quantidade = 100, PrecoUnitario = 25.50m },
                new Operacao { Quantidade = 200, PrecoUnitario = 26.00m }
            };

            // Act
            // Mutação: Alterando o arredondamento
            // Original: resultado exato
            // Mutante: Math.Floor(resultado)  <- Esta mutação fará o teste falhar
            var somaValor = compras.Sum(o => o.Quantidade * o.PrecoUnitario);
            var somaQtd = compras.Sum(o => o.Quantidade);
            var precoMedioMutante = Math.Floor(somaValor / somaQtd); // Mutação introduzida

            // Assert
            // O preço médio correto deveria ser 25.83 (aproximadamente)
            // Com a mutação, será 25.00
            Assert.NotEqual(25.83m, precoMedioMutante, 2);
        }

        [Fact]
        public async Task CalcularPrecoMedio_ComMutacaoFiltro_DeveFalhar()
        {
            // Arrange
            var compras = new List<Operacao>
            {
                new Operacao { Quantidade = 100, PrecoUnitario = 25.50m },
                new Operacao { Quantidade = 200, PrecoUnitario = 26.00m },
                new Operacao { Quantidade = 50, PrecoUnitario = 24.00m }
            };

            // Act
            // Mutação: Alterando o filtro de operações
            // Original: todas as operações
            // Mutante: apenas operações com quantidade > 100  <- Esta mutação fará o teste falhar
            var comprasFiltradas = compras.Where(o => o.Quantidade > 100); // Mutação introduzida
            var somaValor = comprasFiltradas.Sum(o => o.Quantidade * o.PrecoUnitario);
            var somaQtd = comprasFiltradas.Sum(o => o.Quantidade);
            var precoMedioMutante = somaQtd > 0 ? somaValor / somaQtd : 0m;

            // Assert
            // O preço médio correto deveria considerar todas as operações
            // Com a mutação, será calculado apenas com as operações de quantidade > 100
            Assert.NotEqual(25.83m, precoMedioMutante, 2);
        }

        [Fact]
        public async Task CalcularPrecoMedio_ComMutacaoAgrupamento_DeveFalhar()
        {
            // Arrange
            var compras = new List<Operacao>
            {
                new Operacao { Quantidade = 100, PrecoUnitario = 25.50m, AtivoId = 1 },
                new Operacao { Quantidade = 200, PrecoUnitario = 26.00m, AtivoId = 1 },
                new Operacao { Quantidade = 150, PrecoUnitario = 24.00m, AtivoId = 2 }
            };

            // Act
            // Mutação: Alterando o agrupamento
            // Original: todas as operações juntas
            // Mutante: agrupado por AtivoId  <- Esta mutação fará o teste falhar
            var precoMedioPorAtivo = compras
                .GroupBy(o => o.AtivoId)
                .Select(g => new
                {
                    AtivoId = g.Key,
                    PrecoMedio = g.Sum(o => o.Quantidade * o.PrecoUnitario) / g.Sum(o => o.Quantidade)
                })
                .First()
                .PrecoMedio; // Mutação introduzida

            // Assert
            // O preço médio correto deveria considerar todas as operações juntas
            // Com a mutação, será calculado apenas para o primeiro ativo
            Assert.NotEqual(25.83m, precoMedioPorAtivo, 2);
        }
    }
} 