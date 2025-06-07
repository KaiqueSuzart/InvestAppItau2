import React from "react";
import { useLocation, useNavigate, NavLink } from "react-router-dom";
import { useInvestData } from "@/hooks/useInvestData";
import api from "@/services/api";
import { BarChart, Bar, XAxis, Tooltip, ResponsiveContainer } from "recharts";
import {
  BarChart2,
  TrendingUp,
  DollarSign,
  AlertCircle,
  Loader2,
  LogOut,
  RefreshCw
} from "lucide-react";
import { motion } from "framer-motion";
import { PieChart, Pie, Cell, LineChart, Line, YAxis, CartesianGrid, Legend } from "recharts";
import { LineChart as MiniLineChart, Line as MiniLine, ResponsiveContainer as MiniResponsiveContainer, Tooltip as MiniTooltip } from "recharts";

function useQuery() {
  return new URLSearchParams(useLocation().search);
}

export default function DashboardPage() {
  const query = useQuery();
  const navigate = useNavigate();
  const location = useLocation();
  const usuarioIdParam = query.get("usuarioId");
  const usuarioId = usuarioIdParam ? parseInt(usuarioIdParam, 10) : NaN;
  const { totalInvestido, posicaoPorPapel, posicaoGlobal, totalCorretagem, loading, error } = useInvestData(usuarioId);
  const [nomeUsuario, setNomeUsuario] = React.useState<string>("");
  const [lastUpdate, setLastUpdate] = React.useState<Date | null>(null);
  const [refreshing, setRefreshing] = React.useState(false);

  React.useEffect(() => {
    let id = usuarioId;
    if (isNaN(id)) {
      // Tenta recuperar do localStorage
      const storedId = localStorage.getItem('usuarioId');
      if (storedId && !isNaN(Number(storedId))) {
        id = Number(storedId);
        // Redireciona para a mesma página com o usuarioId na URL
        navigate(`${location.pathname}?usuarioId=${id}`, { replace: true });
        return;
      }
      // Se não encontrar, vai para login
      navigate("/");
    } else {
      // Salva no localStorage para persistir entre páginas
      localStorage.setItem('usuarioId', id.toString());
    }
  }, [usuarioId, navigate, location.pathname]);

  // Buscar nome do usuário ao carregar
  React.useEffect(() => {
    if (!isNaN(usuarioId)) {
      api.get(`/invest/usuario/${usuarioId}`)
        .then(res => {
          if (res.data && res.data.nome) setNomeUsuario(res.data.nome);
        })
        .catch(() => setNomeUsuario(""));
    }
  }, [usuarioId]);

  // Função para atualizar dados
  const handleRefresh = async () => {
    setRefreshing(true);
    await new Promise(r => setTimeout(r, 500)); // Simula tempo de fetch
    window.location.reload(); // Simples: recarrega tudo (pode ser melhorado para fetch sem reload)
    setRefreshing(false);
  };

  React.useEffect(() => {
    setLastUpdate(new Date());
  }, [totalInvestido, posicaoPorPapel, posicaoGlobal, totalCorretagem]);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-[#F5F5F5]">
        <Loader2 className="animate-spin w-16 h-16 text-[#004080]" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex flex-col items-center justify-center min-h-screen bg-[#F5F5F5] text-center">
        <AlertCircle className="w-12 h-12 text-red-600 mb-4" />
        <p className="text-xl text-red-600">{error}</p>
        <button
          onClick={() => navigate("/")}
          className="mt-4 bg-[#FF6600] hover:bg-[#e65c00] text-white py-2 px-4 rounded-md transition-colors"
        >
          Voltar ao Login
        </button>
      </div>
    );
  }

  if (totalInvestido.length === 0 && posicaoPorPapel.length === 0 && !posicaoGlobal) {
    return (
      <div className="flex flex-col items-center justify-center min-h-screen bg-[#F5F5F5] text-center">
        <AlertCircle className="w-12 h-12 text-gray-400 mb-4" />
        <p className="text-xl text-gray-500">Nenhuma operação encontrada para este usuário.</p>
        <button
          onClick={() => navigate("/")}
          className="mt-4 bg-[#FF6600] hover:bg-[#e65c00] text-white py-2 px-4 rounded-md transition-colors"
        >
          Voltar ao Login
        </button>
      </div>
    );
  }

  // Dados para gráfico de linha (evolução da carteira)
  const historicoCarteira = posicaoGlobal?.historicoCarteira || [];
  // Dados para gráfico de pizza (distribuição de ativos)
  const distribuicaoAtivos = posicaoGlobal?.distribuicaoAtivos || [];
  const COLORS = ["#004080", "#FF6600", "#0066CC", "#00C49F", "#FFBB28", "#FF8042"];

  return (
    <div className="bg-[#F5F5F5] min-h-screen">
      {/* Header fixo estilizado */}
      <header className="fixed top-0 left-0 w-full h-20 bg-[#004080] flex items-center px-10 shadow-lg z-20 justify-between">
        <div className="flex items-center gap-4">
          <img src="/itau.jpg" alt="Logo" className="h-10 w-auto" style={{ background: '#fff', borderRadius: 8, padding: 2 }} />
          <h1 className="text-2xl font-bold text-white font-inter tracking-wide">Dashboard</h1>
          <nav className="flex gap-8 ml-10">
            <NavLink to={`/dashboard?usuarioId=${usuarioId}`} className={({ isActive }) => isActive ? "text-white underline underline-offset-4" : "text-white/80 hover:text-white hover:scale-105 transition-transform duration-200"}>Dashboard</NavLink>
            <NavLink to={`/operacoes?usuarioId=${usuarioId}`} className={({ isActive }) => isActive ? "text-white underline underline-offset-4" : "text-white/80 hover:text-white hover:scale-105 transition-transform duration-200"}>Operações</NavLink>
            <NavLink to={`/posicoes?usuarioId=${usuarioId}`} className={({ isActive }) => isActive ? "text-white underline underline-offset-4" : "text-white/80 hover:text-white hover:scale-105 transition-transform duration-200"}>Posições</NavLink>
            <NavLink to={`/cotacoes?usuarioId=${usuarioId}`} className={({ isActive }) => isActive ? "text-white underline underline-offset-4" : "text-white/80 hover:text-white hover:scale-105 transition-transform duration-200"}>Cotações</NavLink>
          </nav>
        </div>
        <div className="flex items-center gap-4">
          <span className="text-white text-lg font-medium">{nomeUsuario || "Usuário"}</span>
          <div className="bg-[#e5e7eb] rounded-full w-10 h-10 flex items-center justify-center">
            <svg width="24" height="24" fill="#004080"><circle cx="12" cy="8" r="4"/><ellipse cx="12" cy="18" rx="7" ry="4"/></svg>
          </div>
        </div>
      </header>

      {/* Conteúdo principal: cards, tabela e gráficos */}
      <div className="pt-28 pb-10 max-w-7xl mx-auto px-4">
        {/* Cards de totais */}
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-8 mb-2">
          <div className="bg-white rounded-2xl shadow-md p-8 flex flex-col items-center border-t-4 border-[#004080] w-full">
            <span className="text-gray-500 text-lg mb-2">Carteira Atual</span>
            <span className="text-3xl font-bold text-[#FF6600]">{posicaoGlobal ? posicaoGlobal.valorMercado.toLocaleString("pt-BR", { style: "currency", currency: "BRL" }) : "-"}</span>
            {/* Sparkline */}
            <div className="w-full h-10 mt-2">
              <MiniResponsiveContainer width="100%" height="100%">
                <MiniLineChart data={historicoCarteira.slice(-7)}>
                  <MiniLine type="monotone" dataKey="valor" stroke="#FF6600" strokeWidth={2} dot={false} />
                  <MiniTooltip formatter={(val:number) => val.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })} labelFormatter={d => `Dia: ${d}`} />
                </MiniLineChart>
              </MiniResponsiveContainer>
            </div>
          </div>
          <div className="bg-white rounded-2xl shadow-md p-8 flex flex-col items-center border-t-4 border-[#004080] w-full">
            <span className="text-gray-500 text-lg mb-2">Custo Total</span>
            <span className="text-3xl font-bold text-[#004080]">{posicaoGlobal ? posicaoGlobal.custoTotal.toLocaleString("pt-BR", { style: "currency", currency: "BRL" }) : "-"}</span>
            {/* Sparkline */}
            <div className="w-full h-10 mt-2">
              <MiniResponsiveContainer width="100%" height="100%">
                <MiniLineChart data={historicoCarteira.slice(-7)}>
                  <MiniLine type="monotone" dataKey="valor" stroke="#004080" strokeWidth={2} dot={false} />
                  <MiniTooltip formatter={(val:number) => val.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })} labelFormatter={d => `Dia: ${d}`} />
                </MiniLineChart>
              </MiniResponsiveContainer>
            </div>
          </div>
          <div className="bg-white rounded-2xl shadow-md p-8 flex flex-col items-center border-t-4 border-[#FF6600] w-full">
            <span className="text-gray-500 text-lg mb-2">Lucro/Prejuízo</span>
            <span className={`text-3xl font-bold ${posicaoGlobal && posicaoGlobal.pnL >= 0 ? "text-green-600" : "text-red-600"}`}>{posicaoGlobal ? posicaoGlobal.pnL.toLocaleString("pt-BR", { style: "currency", currency: "BRL" }) : "-"}</span>
            {/* Sparkline */}
            <div className="w-full h-10 mt-2">
              <MiniResponsiveContainer width="100%" height="100%">
                <MiniLineChart data={historicoCarteira.slice(-7)}>
                  <MiniLine type="monotone" dataKey="valor" stroke={posicaoGlobal && posicaoGlobal.pnL >= 0 ? "#16a34a" : "#dc2626"} strokeWidth={2} dot={false} />
                  <MiniTooltip formatter={(val:number) => val.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })} labelFormatter={d => `Dia: ${d}`} />
                </MiniLineChart>
              </MiniResponsiveContainer>
            </div>
          </div>
        </div>
        {/* Última atualização e botão de refresh */}
        <div className="flex items-center gap-4 mb-8 ml-2">
          <span className="text-sm text-gray-500">Última atualização: {lastUpdate ? lastUpdate.toLocaleString("pt-BR") : "-"}</span>
          <button onClick={handleRefresh} className="ml-2 p-2 rounded hover:bg-gray-200 transition-colors" title="Atualizar" disabled={refreshing}>
            <RefreshCw className={`w-5 h-5 ${refreshing ? "animate-spin" : ""}`} />
          </button>
        </div>
        {/* Grid para tabela e gráficos */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-10">
          {/* Tabela de operações */}
          <div className="bg-white rounded-2xl shadow-md p-8">
            <h3 className="text-xl font-semibold text-[#004080] mb-4">Operações</h3>
            <div className="overflow-x-auto">
              <table className="min-w-full text-base">
                <thead>
                  <tr className="bg-[#F5F5F5] text-[#004080]">
                    <th className="px-4 py-2 text-left">Data</th>
                    <th className="px-4 py-2 text-left">Ativo</th>
                    <th className="px-4 py-2 text-left">Tipo</th>
                    <th className="px-4 py-2 text-right">Quantidade</th>
                    <th className="px-4 py-2 text-right">Preço</th>
                  </tr>
                </thead>
                <tbody>
                  {(posicaoGlobal?.operacoes
                    ?.slice()
                    ?.sort((a, b) => new Date(b.data).getTime() - new Date(a.data).getTime())
                    ?.slice(0, 7) || []).map((op, idx) => (
                    <tr key={`${op.id}-${op.data}-${op.tipo}-${idx}`} className="even:bg-gray-100">
                      <td className="px-4 py-2">{new Date(op.data).toLocaleDateString("pt-BR")}</td>
                      <td className="px-4 py-2">{op.ativo}</td>
                      <td className="px-4 py-2">{op.tipo}</td>
                      <td className="px-4 py-2 text-right">{parseInt(op.quantidade.toString(), 10)}</td>
                      <td className="px-4 py-2 text-right">{op.preco.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            {/* Link para ver todas as operações */}
            {posicaoGlobal?.operacoes?.length > 7 && (
              <div className="flex justify-end mt-2">
                <NavLink to={`/operacoes?usuarioId=${usuarioId}`} className="text-[#004080] hover:underline text-sm font-medium">Ver todas →</NavLink>
              </div>
            )}
          </div>
          {/* Gráficos */}
          <div className="flex flex-col gap-8">
            <div className="bg-white rounded-2xl shadow-md p-8 flex-1">
              <h3 className="text-xl font-semibold text-[#004080] mb-4">Valor da Carteira ao longo do tempo</h3>
              <ResponsiveContainer width="100%" height={300}>
                <LineChart data={historicoCarteira} margin={{ top: 10, right: 30, left: 30, bottom: 0 }}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <YAxis tick={{ fill: "#004080" }} width={80} tickFormatter={v => v.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })} />
                  <Tooltip formatter={(val: number) => val.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })} labelFormatter={d => `Data: ${d}`} />
                  <Legend />
                  <Line type="monotone" dataKey="valor" stroke="#004080" strokeWidth={3} dot={false} name="Carteira" />
                </LineChart>
              </ResponsiveContainer>
            </div>
            <div className="bg-white rounded-2xl shadow-md p-8 flex-1" style={{ minHeight: 400, paddingBottom: 80 }}>
              <h3 className="text-xl font-semibold text-[#004080] mb-4">Distribuição de Ativos</h3>
              {distribuicaoAtivos && distribuicaoAtivos.some(a => a.valor > 0) ? (
                <ResponsiveContainer width="100%" height={340}>
                  <PieChart>
                    <Pie 
                      data={distribuicaoAtivos} 
                      dataKey="valor" 
                      nameKey="ativo" 
                      cx="50%" 
                      cy="58%" 
                      outerRadius={120}
                      label={({ cx, cy, midAngle, outerRadius, percent, index }) => {
                        const RADIAN = Math.PI / 180;
                        const radius = outerRadius * 1.25;
                        const x = cx + radius * Math.cos(-midAngle * RADIAN);
                        const y = cy + radius * Math.sin(-midAngle * RADIAN);
                        return (
                          <text
                            x={x}
                            y={y}
                            fill={COLORS[index % COLORS.length]}
                            textAnchor={x > cx ? "start" : "end"}
                            dominantBaseline="central"
                            fontWeight={600}
                          >
                            {`${(percent * 100).toFixed(0)}%`}
                          </text>
                        );
                      }}
                      labelLine={{ length: 30 }}
                      isAnimationActive={false}
                    >
                      {distribuicaoAtivos.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Legend 
                      layout="horizontal" 
                      verticalAlign="bottom" 
                      align="center" 
                      wrapperStyle={{ whiteSpace: "normal", width: "100%", textAlign: "center", marginTop: 120 }}
                    />
                    <Tooltip 
                      formatter={(val, name, props) => `${props.payload.ativo}: R$ ${val.toLocaleString("pt-BR", { minimumFractionDigits: 2 })}`}
                    />
                  </PieChart>
                </ResponsiveContainer>
              ) : (
                <div className="text-gray-400 text-center">Sem dados para exibir</div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
} 