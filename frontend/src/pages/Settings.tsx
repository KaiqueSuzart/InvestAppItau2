import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/Card';
import { Bell, Lock, User, Globe } from 'lucide-react';

const Settings = () => {
  const settings = [
    {
      id: 'notifications',
      title: 'Notificações',
      description: 'Gerencie suas preferências de notificações',
      icon: Bell,
    },
    {
      id: 'security',
      title: 'Segurança',
      description: 'Gerencie suas configurações de segurança',
      icon: Lock,
    },
    {
      id: 'profile',
      title: 'Perfil',
      description: 'Atualize suas informações pessoais',
      icon: User,
    },
    {
      id: 'preferences',
      title: 'Preferências',
      description: 'Configure suas preferências gerais',
      icon: Globe,
    },
  ];

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Configurações</h1>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {settings.map((setting) => (
          <Card key={setting.id} className="cursor-pointer hover:bg-gray-50">
            <CardHeader>
              <div className="flex items-center space-x-4">
                <setting.icon className="h-6 w-6 text-gray-500" />
                <div>
                  <CardTitle>{setting.title}</CardTitle>
                  <p className="text-sm text-gray-500">{setting.description}</p>
                </div>
              </div>
            </CardHeader>
          </Card>
        ))}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Preferências do Sistema</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div>
                <h3 className="text-sm font-medium">Tema Escuro</h3>
                <p className="text-sm text-gray-500">Ativar o tema escuro do sistema</p>
              </div>
              <label className="relative inline-flex items-center cursor-pointer">
                <input type="checkbox" className="sr-only peer" />
                <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
              </label>
            </div>

            <div className="flex items-center justify-between">
              <div>
                <h3 className="text-sm font-medium">Notificações por Email</h3>
                <p className="text-sm text-gray-500">Receber atualizações por email</p>
              </div>
              <label className="relative inline-flex items-center cursor-pointer">
                <input type="checkbox" className="sr-only peer" />
                <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
              </label>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default Settings; 