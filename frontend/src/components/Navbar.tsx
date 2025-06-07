import { Link, useLocation } from 'react-router-dom';
import { Home, PieChart, History, Settings, DollarSign } from 'lucide-react';

const Navbar = () => {
  const location = useLocation();

  // Extrai usuarioId da query string ou do localStorage
  const params = new URLSearchParams(location.search);
  const usuarioId = params.get('usuarioId') || localStorage.getItem('usuarioId');

  // Log para depuração
  console.log('usuarioId Navbar:', usuarioId);

  const isActive = (path: string) => location.pathname === path;

  const navItems = [
    { path: '/dashboard', icon: Home, label: 'Dashboard' },
    { path: '/posicoes', icon: PieChart, label: 'Posições' },
    { path: '/cotacoes', icon: DollarSign, label: 'Cotações' },
    { path: '/portfolio', icon: PieChart, label: 'Portfólio' },
    { path: '/operacoes', icon: History, label: 'Operações' },
    { path: '/settings', icon: Settings, label: 'Configurações' },
  ];

  return (
    <nav className="bg-white shadow-sm">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <div className="flex items-center">
            <span className="text-xl font-bold text-gray-800">InvestApp</span>
          </div>
          <div className="flex space-x-4">
            {navItems.map((item) => (
              <Link
                key={item.path}
                to={usuarioId ? `${item.path}?usuarioId=${usuarioId}` : "#"}
                className={`flex items-center px-3 py-2 rounded-md text-sm font-medium ${
                  isActive(item.path)
                    ? 'bg-gray-100 text-gray-900'
                    : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                } ${!usuarioId ? 'pointer-events-none opacity-50' : ''}`}
                tabIndex={usuarioId ? 0 : -1}
                aria-disabled={!usuarioId}
              >
                <item.icon className="h-5 w-5 mr-2" />
                {item.label}
              </Link>
            ))}
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navbar; 