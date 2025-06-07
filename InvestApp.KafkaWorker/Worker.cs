using Confluent.Kafka;
using Dapper;
using MySql.Data.MySqlClient;
using System.Text.Json;

public class CotacaoKafkaMessage
{
    public int AtivoId { get; set; }
    public decimal PrecoUnitario { get; set; }
    public DateTime DataHora { get; set; }
}

public class Worker : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly ILogger<Worker> _logger;

    public Worker(IConfiguration config, ILogger<Worker> logger)
    {
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var kafkaConfig = new ConsumerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            GroupId = "cotacoes-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(kafkaConfig).Build();
        consumer.Subscribe(_config["Kafka:Topic"]);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var cotacao = JsonSerializer.Deserialize<CotacaoKafkaMessage>(result.Message.Value);

                await SalvarCotacaoComIdempotenciaAsync(cotacao);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Erro ao consumir mensagem do Kafka");
                await Task.Delay(2000, stoppingToken); // Retry simples
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado");
                await Task.Delay(2000, stoppingToken);
            }
        }
    }

    private async Task SalvarCotacaoComIdempotenciaAsync(CotacaoKafkaMessage cotacao)
    {
        var connStr = _config.GetConnectionString("DefaultConnection");
        using var conn = new MySqlConnection(connStr);

        // Idempotência: verifica se já existe cotação para o ativo e data/hora
        var existe = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM cotacoes WHERE ativo_id = @AtivoId AND data_hora = @DataHora",
            new { cotacao.AtivoId, cotacao.DataHora });

        if (existe == 0)
        {
            await conn.ExecuteAsync(
                "INSERT INTO cotacoes (ativo_id, preco_unitario, data_hora) VALUES (@AtivoId, @PrecoUnitario, @DataHora)",
                cotacao);
        }
        // Se já existe, ignora (idempotência)
    }
} 