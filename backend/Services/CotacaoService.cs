using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace InvestApp.Services
{
    public interface ICotacaoService
    {
        Task<decimal?> ObterCotacaoAsync(string ticker);
    }

    public class CotacaoService : ICotacaoService
    {
        private readonly HttpClient _httpClient;
        private readonly CotacaoCache _cache;
        private const string API_KEY = "OGICNIT0PVTWP814"; 
        private const string ALPHA_VANTAGE_API_KEY = "OGICNIT0PVTWP814"; 
        private const string BRAPI_TOKEN = "15fvgPeV2hnsMgW2M2R6nw";

        public CotacaoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _cache = new CotacaoCache(TimeSpan.FromMinutes(5)); // Cache por 5 minutos
        }

        public async Task<decimal?> ObterCotacaoAsync(string ticker)
        {
            Console.WriteLine($"[DEBUG] Buscando cotação para {ticker} (apenas brapi)");
            try
            {
                // Tenta apenas a API da brapi
                var cotacao = await ObterCotacaoBrapiAsync(ticker);
                Console.WriteLine($"[DEBUG] Cotação brapi: {cotacao}");
                if (cotacao.HasValue && cotacao.Value >= 1 && cotacao.Value <= 500)
                {
                    _cache.Adicionar(ticker, cotacao.Value);
                    return cotacao.Value;
                }
                else
                {
                    Console.WriteLine($"[DEBUG] brapi não retornou cotação válida para {ticker}");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Erro ao buscar cotação na brapi: {ex.Message}");
                return null;
            }
        }

        private async Task<decimal?> ObterCotacaoBrapiAsync(string ticker)
        {
            try
            {
                // Remove .SA do ticker se houver
                var tickerBrapi = ticker.Replace(".SA", "");
                var url = $"https://brapi.dev/api/quote/{tickerBrapi}?token={BRAPI_TOKEN}";
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("results", out var results) &&
                    results.ValueKind == JsonValueKind.Array &&
                    results.GetArrayLength() > 0)
                {
                    var result = results[0];
                    if (result.TryGetProperty("regularMarketPrice", out var preco))
                    {
                        return preco.GetDecimal();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Erro brapi: {ex.Message}");
            }
            return null;
        }

        private async Task<decimal?> ObterCotacaoYahooAsync(string ticker)
        {
            try
            {
                var url = $"https://query1.finance.yahoo.com/v7/finance/quote?symbols={ticker}";
                var response = await _httpClient.GetAsync(url);

                if ((int)response.StatusCode == 429)
                {
                    Console.WriteLine($"[CotacaoService] Limite de requisições atingido (429) para Yahoo Finance.");
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("quoteResponse", out var quoteResponse) &&
                    quoteResponse.TryGetProperty("result", out var resultArray) &&
                    resultArray.GetArrayLength() > 0)
                {
                    var result = resultArray[0];
                    if (result.TryGetProperty("regularMarketPrice", out var preco))
                    {
                        return preco.GetDecimal();
                    }
                }
            }
            catch
            {
                // Ignora erro e tenta próxima API
            }

            return null;
        }

        private async Task<decimal?> ObterCotacaoB3Async(string ticker)
        {
            try
            {
                // Remove .SA do ticker para a API da B3
                var tickerB3 = ticker.Replace(".SA", "");
                var url = $"https://api.b3.com.br/v1/cotacao/{tickerB3}";
                
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
                var response = await _httpClient.GetAsync(url);

                if ((int)response.StatusCode == 429)
                {
                    Console.WriteLine($"[CotacaoService] Limite de requisições atingido (429) para B3.");
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("preco", out var preco))
                {
                    return preco.GetDecimal();
                }
            }
            catch
            {
                // Ignora erro e retorna null
            }

            return null;
        }

        private async Task<decimal?> ObterCotacaoAlphaVantageAsync(string ticker)
        {
            try
            {
                // Alpha Vantage usa .SAO para ações brasileiras
                var tickerAlpha = ticker.Replace(".SA", ".SAO");
                var url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={tickerAlpha}&apikey={ALPHA_VANTAGE_API_KEY}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] JSON Alpha Vantage: {json}");
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("Global Quote", out var globalQuote) &&
                    globalQuote.TryGetProperty("05. price", out var preco))
                {
                    if (decimal.TryParse(preco.GetString(), out var valor))
                    {
                        // Se o valor for muito alto, pode estar em centavos, então divide por 100
                        if (valor > 1000)
                        {
                            Console.WriteLine($"[DEBUG] Valor muito alto vindo da Alpha Vantage, dividindo por 100: {valor} -> {valor / 100}");
                            valor = valor / 100;
                        }
                        // Ignora valores fora do intervalo plausível
                        if (valor < 1 || valor > 500)
                        {
                            Console.WriteLine($"[DEBUG] Valor fora do intervalo plausível: {valor}");
                            return null;
                        }
                        return valor;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Erro Alpha Vantage: {ex.Message}");
            }
            return null;
        }
    }
} 