using Xunit;
using Moq;
using InvestApp.Services;
using InvestApp.Models;
using InvestApp.Data;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace InvestApp.UnitTests
{
    public class InvestServiceTests
    {
        [Fact]
        public async Task InserirOperacaoAsync_DeveInserirComSucesso()
        {
            // Arrange
            var repoMock = new Mock<IRepository>();
            var cotacaoMock = new Mock<ICotacaoService>();
            var service = new InvestService(repoMock.Object, cotacaoMock.Object);

            var operacao = new Operacao
            {
                UsuarioId = 1,
                AtivoId = 1,
                TipoOperacao = "COMPRA",
                Quantidade = 10,
                PrecoUnitario = 20,
                DataHora = DateTime.Now
            };

            // Act
            await service.InserirOperacaoAsync(operacao);

            // Assert
            repoMock.Verify(r => r.InserirOperacaoAsync(It.Is<Operacao>(o =>
                o.UsuarioId == 1 &&
                o.AtivoId == 1 &&
                o.TipoOperacao == "COMPRA" &&
                o.Quantidade == 10 &&
                o.PrecoUnitario == 20
            )), Times.Once);
        }

        [Fact]
        public async Task InserirOperacaoAsync_QuantidadeZero_DeveLancarExcecao()
        {
            // Arrange
            var repoMock = new Mock<IRepository>();
            var cotacaoMock = new Mock<ICotacaoService>();
            var service = new InvestService(repoMock.Object, cotacaoMock.Object);

            var operacao = new Operacao
            {
                UsuarioId = 1,
                AtivoId = 1,
                TipoOperacao = "COMPRA",
                Quantidade = 0,
                PrecoUnitario = 20,
                DataHora = DateTime.Now
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.InserirOperacaoAsync(operacao));
        }

        [Fact]
        public async Task GetOperacoesPorUsuarioAsync_ListaVazia_DeveRetornarVazio()
        {
            // Arrange
            var repoMock = new Mock<IRepository>();
            repoMock.Setup(r => r.GetOperacoesPorUsuarioAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Operacao>());
            var cotacaoMock = new Mock<ICotacaoService>();
            var service = new InvestService(repoMock.Object, cotacaoMock.Object);

            // Act
            var result = await service.GetOperacoesPorUsuarioAsync(1);

            // Assert
            Assert.Empty(result);
        }
    }
} 