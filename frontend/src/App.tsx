import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from '@/pages/LoginPage';
import DashboardPage from '@/pages/DashboardPage';
import PortfolioPage from '@/pages/PortfolioPage';
import OperacoesPage from '@/pages/OperacoesPage';
import PosicoesPage from '@/pages/PosicoesPage';
import CotacoesPage from '@/pages/CotacoesPage';
import { Header } from '@/components/Header';

function App() {
  return (
    <Router>
      <div className="min-h-screen bg-gray-100">
        <Header />
        <Routes>
          <Route path="/" element={<LoginPage />} />
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/portfolio" element={<PortfolioPage />} />
          <Route path="/operacoes" element={<OperacoesPage />} />
          <Route path="/posicoes" element={<PosicoesPage />} />
          <Route path="/cotacoes" element={<CotacoesPage />} />
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
