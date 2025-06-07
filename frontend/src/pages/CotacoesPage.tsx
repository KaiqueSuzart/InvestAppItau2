import React, { useEffect, useState } from 'react';
import Navbar from '../components/Navbar';
import api from '../services/api';

interface CotacaoAtivo {
  ativoId: number;
  codigo: string;
  precoAtual: number;
  precoAnterior: number;
  variacao: number;
  variacaoPerc: number;
}

const CotacoesPage: React.FC = () => {
  const [busca, setBusca] = useState('');
  const [cotacoes, setCotacoes] = useState<CotacaoAtivo[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchCotacoes = async () => {
      setLoading(true);
      try {
        // Busca ativos da carteira
        const res = await api.get('/invest/usuario/1/posicaoPorPapel');
        const ativos = res.data;
        // Para cada ativo, busca as duas últimas cotações para calcular variação
        const cotacoesComVaria = await Promise.all(
          ativos.map(async (a: any) => {
            const hist = await api.get(`/invest/ativo/${a.ativoId}/cotacoes`);
            const cotacoesHist = hist.data;
            const precoAtual = cotacoesHist.length ? cotacoesHist[cotacoesHist.length - 1].precoUnitario : a.precoAtual;
            const precoAnterior = cotacoesHist.length > 1 ? cotacoesHist[cotacoesHist.length - 2].precoUnitario : precoAtual;
            const variacao = precoAtual - precoAnterior;
            const variacaoPerc = precoAnterior !== 0 ? ((precoAtual - precoAnterior) / precoAnterior) * 100 : 0;
            return {
              ativoId: a.ativoId,
              codigo: a.codigo,
              precoAtual,
              precoAnterior,
              variacao,
              variacaoPerc,
            };
          })
        );
        setCotacoes(cotacoesComVaria);
      } catch (e) {
        setCotacoes([]);
      } finally {
        setLoading(false);
      }
    };
    fetchCotacoes();
  }, []);

  const cotacoesFiltradas = cotacoes.filter(c =>
    c.codigo.toLowerCase().includes(busca.toLowerCase())
  );

  return (
    <div className="bg-[#F5F5F5] min-h-screen">
      <Navbar />
      <div className="max-w-2xl mx-auto mt-10">
        <div className="bg-white rounded-xl shadow p-6">
          <input
            type="text"
            placeholder="Buscar ativo..."
            value={busca}
            onChange={e => setBusca(e.target.value)}
            className="mb-4 w-full px-4 py-2 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-[#004080] bg-gray-50"
          />
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b">
                  <th className="text-left py-2 px-2 text-gray-700 font-semibold">Ativo</th>
                  <th className="text-right py-2 px-2 text-gray-700 font-semibold">Últ. Preço</th>
                  <th className="text-right py-2 px-2 text-gray-700 font-semibold">Variação</th>
                  <th className="text-right py-2 px-2 text-gray-700 font-semibold">Variação %</th>
                </tr>
              </thead>
              <tbody>
                {loading ? (
                  <tr><td colSpan={4} className="text-center py-8 text-[#004080]">Carregando...</td></tr>
                ) : cotacoesFiltradas.length === 0 ? (
                  <tr><td colSpan={4} className="text-center py-8 text-gray-400">Nenhum ativo encontrado</td></tr>
                ) : (
                  cotacoesFiltradas.map((c) => (
                    <tr key={c.ativoId} className="border-b hover:bg-gray-50">
                      <td className="py-2 px-2 font-bold text-gray-800">{c.codigo}</td>
                      <td className="py-2 px-2 text-right">R$ {c.precoAtual.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}</td>
                      <td className={`py-2 px-2 text-right font-medium ${c.variacao >= 0 ? 'text-green-600' : 'text-red-600'}`}>{c.variacao >= 0 ? '+' : ''}{c.variacao.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}</td>
                      <td className={`py-2 px-2 text-right font-medium ${c.variacaoPerc >= 0 ? 'text-green-600' : 'text-red-600'}`}>{c.variacaoPerc >= 0 ? '+' : ''}{c.variacaoPerc.toFixed(2)}%</td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CotacoesPage; 