import { useState, useEffect } from 'react';
import api from '@/services/api';

export interface HistoricoIndicadoresItem {
  data: string;
  custoTotal: number;
  pnL: number;
  corretagem: number;
}

export function useHistoricoIndicadores(usuarioId: number) {
  const [historico, setHistorico] = useState<HistoricoIndicadoresItem[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    api.get(`/invest/usuario/${usuarioId}/historicoIndicadores`)
      .then(res => setHistorico(res.data))
      .finally(() => setLoading(false));
  }, [usuarioId]);

  return { historico, loading };
} 