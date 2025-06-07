using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace InvestApp.Services
{
    public class CotacaoCache
    {
        private readonly ConcurrentDictionary<string, (decimal Preco, DateTime DataAtualizacao)> _cache;
        private readonly TimeSpan _tempoExpiracao;

        public CotacaoCache(TimeSpan tempoExpiracao)
        {
            _cache = new ConcurrentDictionary<string, (decimal, DateTime)>();
            _tempoExpiracao = tempoExpiracao;
        }

        public void Adicionar(string ticker, decimal preco)
        {
            _cache[ticker] = (preco, DateTime.Now);
        }

        public decimal? Obter(string ticker)
        {
            if (_cache.TryGetValue(ticker, out var valor))
            {
                if (DateTime.Now - valor.DataAtualizacao <= _tempoExpiracao)
                {
                    return valor.Preco;
                }
                _cache.TryRemove(ticker, out _);
            }
            return null;
        }
    }
} 