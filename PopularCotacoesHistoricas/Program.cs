using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Tools
{
    class PopularCotacoesHistoricas
    {
        private const string ConnectionString = "Server=localhost;Database=investdb;User Id=root;Password=Al101299*;";
        private const string BrapiToken = "15fvgPeV2hnsMgW2M2R6nw";
        private static readonly string[] Tickers = { "ITSA3", "PETR4", "VALE3", "ABEV3", "MGLU3" };
        private const string Range = "1y"; // 1 ano
        private const string Interval = "1d";

        static async Task Main(string[] args)
        {
            using var httpClient = new HttpClient();
            using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();

            foreach (var ticker in Tickers)
            {
                Console.WriteLine($"Buscando histórico para {ticker}...");
                var url = $"https://brapi.dev/api/quote/HISTORICAL/{ticker}?range={Range}&interval={Interval}&token={BrapiToken}";
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Erro ao buscar {ticker}: {response.StatusCode}");
                    continue;
                }
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("results", out var results) &&
                    results.ValueKind == JsonValueKind.Array &&
                    results.GetArrayLength() > 0)
                {
                    var result = results[0];
                    if (result.TryGetProperty("historicalDataPrice", out var historico) && historico.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var dia in historico.EnumerateArray())
                        {
                            if (dia.TryGetProperty("close", out var preco) && preco.ValueKind == JsonValueKind.Number &&
                                dia.TryGetProperty("date", out var data) && data.ValueKind == JsonValueKind.String)
                            {
                                var precoDecimal = preco.GetDecimal();
                                var dataStr = data.GetString();
                                if (DateTime.TryParse(dataStr, out var dataCotacao))
                                {
                                    // Descobrir o id do ativo no banco
                                    var cmdAtivo = new MySqlCommand("SELECT id FROM ativos WHERE codigo = @codigo", connection);
                                    cmdAtivo.Parameters.AddWithValue("@codigo", ticker);
                                    var ativoIdObj = await cmdAtivo.ExecuteScalarAsync();
                                    if (ativoIdObj == null) continue;
                                    int ativoId = Convert.ToInt32(ativoIdObj);

                                    // Verificar se já existe cotação para esse ativo/data
                                    var cmdCheck = new MySqlCommand("SELECT COUNT(*) FROM cotacoes WHERE ativo_id = @ativoId AND data_hora = @data", connection);
                                    cmdCheck.Parameters.AddWithValue("@ativoId", ativoId);
                                    cmdCheck.Parameters.AddWithValue("@data", dataCotacao);
                                    var exists = Convert.ToInt32(await cmdCheck.ExecuteScalarAsync()) > 0;
                                    if (exists) continue;

                                    // Inserir cotação
                                    var cmd = new MySqlCommand("INSERT INTO cotacoes (ativo_id, preco_unitario, data_hora) VALUES (@ativoId, @preco, @data)", connection);
                                    cmd.Parameters.AddWithValue("@ativoId", ativoId);
                                    cmd.Parameters.AddWithValue("@preco", precoDecimal);
                                    cmd.Parameters.AddWithValue("@data", dataCotacao);
                                    await cmd.ExecuteNonQueryAsync();
                                    Console.WriteLine($"Inserido: {ticker} {dataCotacao:yyyy-MM-dd} R$ {precoDecimal}");
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Processo finalizado!");
        }
    }
} 