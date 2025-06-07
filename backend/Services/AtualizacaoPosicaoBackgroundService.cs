using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using InvestApp.Data;

namespace InvestApp.Services
{
    public class AtualizacaoPosicaoBackgroundService : BackgroundService
    {
        private readonly IPosicaoService _posicaoService;
        private readonly IRepository _repositorio;

        public AtualizacaoPosicaoBackgroundService(IPosicaoService posicaoService, IRepository repositorio)
        {
            _posicaoService = posicaoService;
            _repositorio = repositorio;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _posicaoService.AtualizarTodasPosicoesAsync();
                }
                catch (Exception ex)
                {
                    // Log do erro
                    Console.WriteLine($"Erro ao atualizar posições: {ex.Message}");
                }

                // Aguarda 1 hora antes de rodar novamente
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
} 