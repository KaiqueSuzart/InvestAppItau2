import React, { useEffect, useState, useRef } from 'react';
import TablePosicao from '../components/TablePosicao';
import { Card } from '../components/ui/Card';
import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip, Legend } from 'recharts';
import api from '../services/api';
import { useLocation } from 'react-router-dom';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip as RechartsTooltip, Legend as RechartsLegend, ResponsiveContainer as RechartsResponsiveContainer } from 'recharts';

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884d8'];

const INDICES_OPCOES = [
  { key: 'Ibovespa', label: 'Ibovespa', color: '#FF6600' },
  { key: 'CDI', label: 'CDI', color: '#00C49F' },
  { key: 'Inflação', label: 'Inflação', color: '#FFBB28' },
  { key: 'Dólar', label: 'Dólar', color: '#8884d8' },
];

function useUsuarioId() {
  const location = useLocation();
  const params = new URLSearchParams(location.search);
  return params.get('usuarioId') || localStorage.getItem('usuarioId') || '';
}

const PosicoesPage: React.FC = () => {
  const usuarioId = useUsuarioId();
  const [positions, setPositions] = useState<any[]>([]);
  const [pieData, setPieData] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [historicoCarteira, setHistoricoCarteira] = useState<any[]>([]);
  const [indicesSelecionados, setIndicesSelecionados] = useState<string[]>(['Ibovespa', 'CDI', 'Inflação', 'Dólar']);
  const [dataInicio, setDataInicio] = useState<string>("");
  const [dataFim, setDataFim] = useState<string>("");
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Fecha o dropdown ao clicar fora
  React.useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setDropdownOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  useEffect(() => {
    if (!usuarioId) {
      setError('Usuário não identificado.');
      setLoading(false);
      return;
    }
    setLoading(true);
    setError(null);
    Promise.all([
      api.get(`/invest/usuario/${usuarioId}/posicaoPorPapel`),
      api.get(`/invest/usuario/${usuarioId}/posicaoGlobal`)
    ])
      .then(([posRes, globalRes]) => {
        setPositions(
          posRes.data
            .map((p: any, idx: number) => ({
              id: p.ativoId || idx,
              codigo: p.codigo,
              quantity: p.quantidadeLiquida,
              averagePrice: p.precoMedio,
              currentPrice: p.precoAtual,
              profit: (p.quantidadeLiquida * (p.precoAtual - p.precoMedio)),
            }))
            .sort((a, b) => (b.quantity * b.currentPrice) - (a.quantity * a.currentPrice))
        );
        setPieData(
          (globalRes.data.distribuicaoAtivos || []).map((a: any) => ({
            name: a.ativo,
            value: a.valor
          }))
        );
        setHistoricoCarteira(globalRes.data.historicoCarteira || []);
      })
      .catch(() => setError('Erro ao buscar dados das posições.'))
      .finally(() => setLoading(false));
  }, [usuarioId]);

  if (loading) return <div className="flex justify-center items-center h-96">Carregando...</div>;
  if (error) return <div className="flex justify-center items-center h-96 text-red-600">{error}</div>;

  return (
    <div className="flex flex-col w-full min-h-screen bg-gray-50 pt-20">
      <div className="flex flex-col items-center w-full">
        <div className="w-full max-w-4xl flex flex-col gap-8">
          {/* Tabela em cima */}
          <Card className="w-full">
            <div className="p-6">
              <h2 className="text-lg font-semibold mb-4 text-[#004080]">Posições</h2>
              <TablePosicao positions={positions} />
            </div>
          </Card>
          {/* Gráfico embaixo */}
          <Card className="w-full">
            <div className="p-6 flex flex-col items-center">
              <h2 className="text-lg font-semibold mb-4 text-[#004080]">Distribuição de Ativos</h2>
              <div className="h-72 w-full">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={pieData}
                      cx="50%"
                      cy="50%"
                      labelLine={false}
                      outerRadius={100}
                      fill="#8884d8"
                      dataKey="value"
                      label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                    >
                      {pieData.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip formatter={(value: number) => `R$ ${value.toFixed(2)}`} />
                    <Legend />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            </div>
          </Card>
          {/* Card do gráfico de rendimento mensal */}
          <Card className="w-full">
            <div className="p-6 flex flex-col items-center">
              <h2 className="text-lg font-semibold mb-4 text-[#004080]">Rendimento no mês vs Índices</h2>
              {/* Filtros */}
              <div className="flex flex-wrap gap-4 mb-4 w-full justify-center items-end">
                <div>
                  <label className="block text-sm text-gray-700 mb-1">Comparar com:</label>
                  <div className="relative" ref={dropdownRef}>
                    <button
                      type="button"
                      className="border rounded-md px-3 py-2 w-48 text-left bg-white focus:outline-none focus:ring-2 focus:ring-[#004080] flex justify-between items-center"
                      onClick={() => setDropdownOpen((v) => !v)}
                    >
                      <span className="truncate">
                        {indicesSelecionados.length === 0
                          ? 'Selecione os índices'
                          : indicesSelecionados.map(k => INDICES_OPCOES.find(i => i.key === k)?.label).join(', ')}
                      </span>
                      <svg className={`w-4 h-4 ml-2 transition-transform ${dropdownOpen ? 'rotate-180' : ''}`} fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" /></svg>
                    </button>
                    {dropdownOpen && (
                      <div className="absolute z-10 mt-1 w-48 bg-white border rounded-md shadow-lg max-h-52 overflow-auto">
                        {INDICES_OPCOES.map(idx => (
                          <label key={idx.key} className="flex items-center gap-2 px-3 py-2 hover:bg-gray-100 cursor-pointer text-sm">
                            <input
                              type="checkbox"
                              checked={indicesSelecionados.includes(idx.key)}
                              onChange={e => {
                                if (e.target.checked) {
                                  setIndicesSelecionados([...indicesSelecionados, idx.key]);
                                } else {
                                  setIndicesSelecionados(indicesSelecionados.filter(k => k !== idx.key));
                                }
                              }}
                              className="accent-[#004080]"
                            />
                            {idx.label}
                          </label>
                        ))}
                      </div>
                    )}
                  </div>
                </div>
                <div>
                  <label className="block text-sm text-gray-700 mb-1">Data início:</label>
                  <input
                    type="date"
                    value={dataInicio}
                    onChange={e => setDataInicio(e.target.value)}
                    className="border rounded-md px-3 py-2 w-36 focus:outline-none focus:ring-2 focus:ring-[#004080]"
                  />
                </div>
                <div>
                  <label className="block text-sm text-gray-700 mb-1">Data fim:</label>
                  <input
                    type="date"
                    value={dataFim}
                    onChange={e => setDataFim(e.target.value)}
                    className="border rounded-md px-3 py-2 w-36 focus:outline-none focus:ring-2 focus:ring-[#004080]"
                  />
                </div>
              </div>
              <div className="h-72 w-full">
                <RechartsResponsiveContainer width="100%" height="100%">
                  <LineChart data={getComparativoMensal(historicoCarteira, indicesSelecionados, dataInicio, dataFim)}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="data" />
                    <YAxis tickFormatter={v => v + '%'} />
                    <RechartsTooltip formatter={(v: number) => v.toFixed(2) + '%'} />
                    <RechartsLegend />
                    <Line type="monotone" dataKey="Carteira" stroke="#004080" strokeWidth={2} dot={false} />
                    {INDICES_OPCOES.filter(idx => indicesSelecionados.includes(idx.key)).map(idx => (
                      <Line
                        key={idx.key}
                        type="monotone"
                        dataKey={idx.key}
                        stroke={idx.color}
                        strokeWidth={2}
                        dot={false}
                      />
                    ))}
                  </LineChart>
                </RechartsResponsiveContainer>
              </div>
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
};

// Função utilitária para montar o comparativo mensal
function getComparativoMensal(historico: any[], indicesSelecionados: string[], dataInicio: string, dataFim: string) {
  // Mock dos índices para o mês (exemplo: 5 pontos)
  const indices = [
    { data: '2024-06-01', Ibovespa: 0, CDI: 0, Inflação: 0, Dólar: 0 },
    { data: '2024-06-07', Ibovespa: 1.2, CDI: 0.7, Inflação: 0.3, Dólar: 0.5 },
    { data: '2024-06-14', Ibovespa: 2.1, CDI: 1.1, Inflação: 0.6, Dólar: 1.2 },
    { data: '2024-06-21', Ibovespa: 1.8, CDI: 1.3, Inflação: 0.8, Dólar: 2.0 },
    { data: '2024-06-28', Ibovespa: 2.5, CDI: 1.5, Inflação: 1.0, Dólar: 2.7 },
  ];
  // Pega os dados do histórico da carteira no mês (mock: pega últimos 5 pontos)
  let carteira = (historico || []).slice(-5).map((h, i, arr) => {
    let rendimento = 0;
    if (i > 0 && arr[i-1].valor) {
      rendimento = ((h.valor - arr[i-1].valor) / arr[i-1].valor) * 100;
    }
    return {
      data: indices[i]?.data || h.data || `${i+1}`,
      Carteira: Number(rendimento.toFixed(2))
    };
  });
  // Junta os dados
  let result = carteira.map((c, i) => ({
    ...c,
    ...Object.fromEntries(INDICES_OPCOES.map(idx => [idx.key, indices[i]?.[idx.key] ?? 0]))
  }));
  // Filtro de datas
  if (dataInicio) result = result.filter(r => r.data >= dataInicio);
  if (dataFim) result = result.filter(r => r.data <= dataFim);
  // Mostra só os índices selecionados (a linha do gráfico já filtra)
  return result;
}

export default PosicoesPage; 