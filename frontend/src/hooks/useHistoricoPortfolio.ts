import { useState, useEffect } from 'react';
import api from '@/services/api';

export interface HistoricoPortfolioItem {
  data: string;
  valorCarteira: number;
}

export function useHistoricoPortfolio(usuarioId: number) {
  const [historico, setHistorico] = useState<HistoricoPortfolioItem[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    api.get(`/invest/usuario/${usuarioId}/historicoPortfolio`)
      .then(res => setHistorico(res.data))
      .finally(() => setLoading(false));
  }, [usuarioId]);

  return { historico, loading };
} 