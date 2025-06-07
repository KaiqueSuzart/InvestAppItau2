using InvestApp.Services.DTOs;

namespace InvestApp.Services
{
    public class PrecoMedioService : IPrecoMedioService
    {
        public PrecoMedioService()
        {
        }

        public decimal CalcularPrecoMedioPonderado(List<OperacaoDTO> operacoes)
        {
            if (operacoes == null || !operacoes.Any())
            {
                throw new ArgumentException("A lista de operações não pode ser nula ou vazia.");
            }

            var operacoesValidas = operacoes.Where(o => o.TipoOperacao == "COMPRA").ToList();

            if (!operacoesValidas.Any())
            {
                throw new ArgumentException("Não há operações de compra válidas.");
            }

            foreach (var operacao in operacoesValidas)
            {
                if (operacao.Quantidade <= 0)
                {
                    throw new ArgumentException("A quantidade deve ser maior que zero.");
                }

                if (operacao.Preco <= 0)
                {
                    throw new ArgumentException("O preço deve ser maior que zero.");
                }
            }

            decimal valorTotal = operacoesValidas.Sum(o => o.Quantidade * o.Preco);
            int quantidadeTotal = operacoesValidas.Sum(o => o.Quantidade);

            return valorTotal / quantidadeTotal;
        }
    }
} 