import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/Card';
import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from 'recharts';

const Portfolio = () => {
  // Dados mockados para exemplo
  const investments = [
    { id: 1, name: 'PETR4', quantity: 100, averagePrice: 28.50, currentPrice: 30.20, profit: 170 },
    { id: 2, name: 'VALE3', quantity: 50, averagePrice: 65.80, currentPrice: 68.90, profit: 155 },
    { id: 3, name: 'ITUB4', quantity: 200, averagePrice: 22.30, currentPrice: 23.15, profit: 170 },
  ];

  const pieData = [
    { name: 'PETR4', value: 3020 },
    { name: 'VALE3', value: 3445 },
    { name: 'ITUB4', value: 4630 },
  ];

  const COLORS = ['#0088FE', '#00C49F', '#FFBB28'];

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Portfólio</h1>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Distribuição do Portfólio</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-[300px]">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={pieData}
                    cx="50%"
                    cy="50%"
                    labelLine={false}
                    outerRadius={80}
                    fill="#8884d8"
                    dataKey="value"
                    label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                  >
                    {pieData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Investimentos</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b">
                    <th className="text-left py-3 px-4">Ativo</th>
                    <th className="text-right py-3 px-4">Quantidade</th>
                    <th className="text-right py-3 px-4">Preço Médio</th>
                    <th className="text-right py-3 px-4">Preço Atual</th>
                    <th className="text-right py-3 px-4">Lucro/Prejuízo</th>
                  </tr>
                </thead>
                <tbody>
                  {investments.map((investment) => (
                    <tr key={investment.id} className="border-b">
                      <td className="py-3 px-4">{investment.name}</td>
                      <td className="text-right py-3 px-4">{investment.quantity}</td>
                      <td className="text-right py-3 px-4">R$ {investment.averagePrice.toFixed(2)}</td>
                      <td className="text-right py-3 px-4">R$ {investment.currentPrice.toFixed(2)}</td>
                      <td className={`text-right py-3 px-4 ${investment.profit >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                        R$ {investment.profit.toFixed(2)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default Portfolio; 