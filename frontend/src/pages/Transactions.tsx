import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/Card';
import { ArrowUp, ArrowDown } from 'lucide-react';

const Transactions = () => {
  // Dados mockados para exemplo
  const transactions = [
    {
      id: 1,
      type: 'compra',
      asset: 'PETR4',
      quantity: 100,
      price: 28.50,
      date: '2024-03-15',
      total: 2850,
    },
    {
      id: 2,
      type: 'venda',
      asset: 'VALE3',
      quantity: 50,
      price: 68.90,
      date: '2024-03-14',
      total: 3445,
    },
    {
      id: 3,
      type: 'compra',
      asset: 'ITUB4',
      quantity: 200,
      price: 22.30,
      date: '2024-03-13',
      total: 4460,
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Transações</h1>
        <button className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700">
          Nova Transação
        </button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Histórico de Transações</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b">
                  <th className="text-left py-3 px-4">Data</th>
                  <th className="text-left py-3 px-4">Tipo</th>
                  <th className="text-left py-3 px-4">Ativo</th>
                  <th className="text-right py-3 px-4">Quantidade</th>
                  <th className="text-right py-3 px-4">Preço</th>
                  <th className="text-right py-3 px-4">Total</th>
                </tr>
              </thead>
              <tbody>
                {transactions.map((transaction) => (
                  <tr key={transaction.id} className="border-b">
                    <td className="py-3 px-4">{transaction.date}</td>
                    <td className="py-3 px-4">
                      <div className="flex items-center">
                        {transaction.type === 'compra' ? (
                          <ArrowUp className="h-4 w-4 text-green-600 mr-2" />
                        ) : (
                          <ArrowDown className="h-4 w-4 text-red-600 mr-2" />
                        )}
                        <span className="capitalize">{transaction.type}</span>
                      </div>
                    </td>
                    <td className="py-3 px-4">{transaction.asset}</td>
                    <td className="text-right py-3 px-4">{transaction.quantity}</td>
                    <td className="text-right py-3 px-4">R$ {transaction.price.toFixed(2)}</td>
                    <td className="text-right py-3 px-4">R$ {transaction.total.toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default Transactions; 