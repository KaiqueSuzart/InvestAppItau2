import React from "react";
import { useLocation, useNavigate, NavLink } from "react-router-dom";
import { useInvestData } from "@/hooks/useInvestData";
import { PieChart, Pie, Cell, Tooltip, Legend, ResponsiveContainer } from "recharts";
import { ArrowLeft, LogOut, AlertCircle } from "lucide-react";
import { motion } from "framer-motion";
import { useHistoricoPortfolio } from '@/hooks/useHistoricoPortfolio';
import { LineChart, Line, XAxis, YAxis, ResponsiveContainer as RechartsResponsiveContainer } from 'recharts';

function useQuery() {
  return new URLSearchParams(useLocation().search);
}

function EvolucaoPortfolioChart({ usuarioId }: { usuarioId: number }) {
  const { historico, loading } = useHistoricoPortfolio(usuarioId);

  if (loading) {
    return <div>Carregando histórico...</div>;
  }

  if (!historico.length) {
    return <div>Nenhum dado de histórico disponível.</div>;
  }

  const valorInicial = historico[0].valorCarteira;
  const valorFinal = historico[historico.length - 1].valorCarteira;
  const rendimento = valorFinal - valorInicial;
  const rendimentoPercentual = valorInicial > 0 ? ((valorFinal / valorInicial) - 1) * 100 : 0;

  return (
    <div>
      <div className="mb-4">
        <p>Meus Investimentos: <b>R$ {valorFinal.toLocaleString("pt-BR", { minimumFractionDigits: 2 })}</b></p>
        <p>
          Rendeu: <span style={{ color: rendimento >= 0 ? 'green' : 'red' }}>
            {rendimentoPercentual.toFixed(2)}% ({rendimento >= 0 ? '+' : ''}R$ {rendimento.toFixed(2)})
          </span>
        </p>
      </div>
      <RechartsResponsiveContainer width="100%" height={250}>
        <LineChart data={historico}>
          <XAxis dataKey="data" />
          <YAxis />
          <Tooltip formatter={val => val.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })} />
          <Legend />
          <Line type="monotone" dataKey="valorCarteira" stroke="#00C853" name="Carteira" />
        </LineChart>
      </RechartsResponsiveContainer>
    </div>
  );
}

export default function PortfolioPage() {
  const query = useQuery();
  const navigate = useNavigate();
  const usuarioIdParam = query.get("usuarioId");
  const usuarioId = usuarioIdParam ? parseInt(usuarioIdParam, 10) : NaN;
  const { totalInvestido, posicaoPorPapel, loading, error } = useInvestData(usuarioId);

  React.useEffect(() => {
    if (isNaN(usuarioId)) {
      navigate("/");
    }
  }, [usuarioId, navigate]);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-[#F5F5F5]">
        <motion.div animate={{ rotate: 360 }} transition={{ repeat: Infinity, duration: 1 }}>
          <ArrowLeft className="w-16 h-16 text-[#004080]" />
        </motion.div>
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
          className="mt-4 bg-[#FF6600] hover:bg-[#e65c00] text-white py-2 px-4 rounded-md"
        >
          Voltar ao Login
        </button>
      </div>
    );
  }

  const pieData = totalInvestido.map(item => ({
    name: item.codigo,
    value: item.totalInvestido
  }));
  const COLORS = ["#FF6600", "#0066CC", "#004080", "#00A651", "#FFCC00", "#9933CC"];

  return (
    <div className="bg-[#F5F5F5] min-h-screen pt-20 pb-10">
      {/* Header igual ao Dashboard */}
      <header className="fixed top-0 left-0 w-full h-16 bg-[#004080] flex items-center px-6 shadow-lg z-20">
        <button onClick={() => navigate(-1)} className="text-white hover:text-gray-200 mr-4 transition-colors">
          <ArrowLeft className="w-6 h-6" />
        </button>
        <h1 className="text-xl font-bold text-white font-inter">InvestApp</h1>
        <nav className="hidden sm:flex space-x-6 ml-8">
          <NavLink
            to={`/dashboard?usuarioId=${usuarioId}`}
            className={({ isActive }) =>
              isActive ? "text-white underline underline-offset-4" : "text-white/80 hover:text-white hover:scale-105 transition-transform duration-200"
            }
          >
            Dashboard
          </NavLink>
          <NavLink
            to={`/portfolio?usuarioId=${usuarioId}`}
            className={({ isActive }) =>
              isActive ? "text-white underline underline-offset-4" : "text-white/80 hover:text-white hover:scale-105 transition-transform duration-200"
            }
          >
            Portfólio
          </NavLink>
        </nav>
        <button
          onClick={() => navigate("/")}
          className="ml-auto text-white hover:text-gray-200 flex items-center transition-colors"
        >
          <LogOut className="w-5 h-5 mr-1" />
          <span>Sair</span>
        </button>
      </header>

      <div className="max-w-7xl mx-auto px-4">
        <motion.h2
          className="text-3xl font-semibold text-[#004080] mb-6"
          initial={{ opacity: 0, y: -10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
        >
          Portfólio InvestApp
        </motion.h2>

        {/* PieChart de Total Investido */}
        <div className="bg-white rounded-2xl shadow-md p-6 mb-8">
          <h3 className="text-2xl font-semibold text-[#004080] mb-4">Distribuição de Investimento</h3>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={pieData}
                dataKey="value"
                nameKey="name"
                cx="50%"
                cy="50%"
                outerRadius={100}
                fill="#FF6600"
                label={({ name, percent }) => `${name} (${(percent * 100).toFixed(0)}%)`}
              >
                {pieData.map((_, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip
                formatter={(value: number) => value.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })}
              />
              <Legend verticalAlign="bottom" iconSize={12} />
            </PieChart>
          </ResponsiveContainer>
        </div>

        {/* Gráfico de Evolução do Portfólio */}
        <div className="bg-white rounded-2xl shadow-md p-6 mb-8">
          <h3 className="text-2xl font-semibold text-[#004080] mb-4">Evolução do Portfólio</h3>
          <EvolucaoPortfolioChart usuarioId={usuarioId} />
        </div>

        {/* Tabela de Detalhes do Portfólio */}
        <div className="bg-white rounded-2xl shadow-md p-6 mb-8">
          <h3 className="text-2xl font-semibold text-[#004080] mb-4">Detalhes do Portfólio</h3>
          <div className="overflow-x-auto">
            <table className="min-w-full">
              <thead>
                <tr className="bg-[#004080] text-white">
                  <th className="px-4 py-2 text-left">Ativo</th>
                  <th className="px-4 py-2 text-right">Total Investido</th>
                  <th className="px-4 py-2 text-right">Quantidade</th>
                  <th className="px-4 py-2 text-right">Preço Médio</th>
                </tr>
              </thead>
              <tbody>
                {totalInvestido.map(item => {
                  const pos = posicaoPorPapel.find(p => p.ativoId === item.ativoId);
                  return (
                    <tr key={item.ativoId} className="even:bg-gray-100">
                      <td className="px-4 py-2">{item.codigo}</td>
                      <td className="px-4 py-2 text-right">
                        {item.totalInvestido.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })}
                      </td>
                      <td className="px-4 py-2 text-right">
                        {pos ? pos.quantidadeLiquida.toFixed(2) : "--"}
                      </td>
                      <td className="px-4 py-2 text-right">
                        {pos && pos.precoMedio
                          ? pos.precoMedio.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })
                          : "--"}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
} 