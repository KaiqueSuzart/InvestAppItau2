import { useState, useEffect } from 'react';
import api from '@/services/api';

export interface TotalInvestido {
  ativoId: number;
  totalInvestido: number;
}

export interface PosicaoPorPapel {
  ativoId: number;
  codigo: string;
  quantidadeLiquida: number;
  precoMedio: number;
}

export interface PosicaoGlobal {
  usuarioId: number;
  valorMercado: number;
  custoTotal: number;
  pnL: number;
}

interface HookResponse {
  totalInvestido: TotalInvestido[];
  posicaoPorPapel: PosicaoPorPapel[];
  posicaoGlobal: PosicaoGlobal | null;
  totalCorretagem: number;
  loading: boolean;
  error: string | null;
}

export function useInvestData(usuarioId: number): HookResponse {
  const [totalInvestido, setTotalInvestido] = useState<TotalInvestido[]>([]);
  const [posicaoPorPapel, setPosicaoPorPapel] = useState<PosicaoPorPapel[]>([]);
  const [posicaoGlobal, setPosicaoGlobal] = useState<PosicaoGlobal | null>(null);
  const [totalCorretagem, setTotalCorretagem] = useState<number>(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;
    async function fetchData() {
      try {
        const [tiResp, ppResp, pgResp, tcResp] = await Promise.all([
          api.get(`/invest/usuario/${usuarioId}/totalInvestido`),
          api.get(`/invest/usuario/${usuarioId}/posicaoPorPapel`),
          api.get(`/invest/usuario/${usuarioId}/posicaoGlobal`),
          api.get(`/invest/usuario/${usuarioId}/totalCorretagem`)
        ]);
        if (!isMounted) return;
        setTotalInvestido(Array.isArray(tiResp.data) ? tiResp.data : []);
        setPosicaoPorPapel(Array.isArray(ppResp.data) ? ppResp.data : []);
        if (pgResp.data && typeof pgResp.data === 'object' && 'mensagem' in pgResp.data) {
          setError(pgResp.data.mensagem);
          setPosicaoGlobal(null);
        } else {
          setPosicaoGlobal(pgResp.data ?? null);
        }
        setTotalCorretagem(typeof tcResp.data === "number" ? tcResp.data : 0);
      } catch (err: any) {
        if (!isMounted) return;
        if (err.response?.status === 204 || err.response?.status === 404) {
          setTotalInvestido([]);
          setPosicaoPorPapel([]);
          setPosicaoGlobal(null);
          setTotalCorretagem(0);
        } else {
          setError(err.response?.data?.message || err.message);
        }
      } finally {
        if (!isMounted) return;
        setLoading(false);
      }
    }
    fetchData();
    return () => { isMounted = false; };
  }, [usuarioId]);

  return { totalInvestido, posicaoPorPapel, posicaoGlobal, totalCorretagem, loading, error };
} 