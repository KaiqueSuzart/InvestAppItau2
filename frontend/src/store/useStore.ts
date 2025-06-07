import { create } from 'zustand';
import api from '@/services/api';

interface Investment {
  id: number;
  name: string;
  quantity: number;
  averagePrice: number;
  currentPrice: number;
  profit: number;
}

interface Transaction {
  id: number;
  type: 'compra' | 'venda';
  asset: string;
  quantity: number;
  price: number;
  date: string;
  total: number;
}

interface Store {
  investments: Investment[];
  transactions: Transaction[];
  portfolioValue: number;
  dailyChange: number;
  monthlyChange: number;
  totalInvested: number;
  isLoading: boolean;
  error: string | null;
  fetchInvestments: () => Promise<void>;
  fetchTransactions: () => Promise<void>;
  addTransaction: (transaction: Omit<Transaction, 'id'>) => Promise<void>;
  updatePortfolio: () => Promise<void>;
}

const useStore = create<Store>((set, get) => ({
  investments: [],
  transactions: [],
  portfolioValue: 0,
  dailyChange: 0,
  monthlyChange: 0,
  totalInvested: 0,
  isLoading: false,
  error: null,

  fetchInvestments: async () => {
    try {
      set({ isLoading: true, error: null });
      const response = await api.get('/investments');
      set({ investments: response.data });
    } catch (error) {
      set({ error: 'Erro ao carregar investimentos' });
    } finally {
      set({ isLoading: false });
    }
  },

  fetchTransactions: async () => {
    try {
      set({ isLoading: true, error: null });
      const response = await api.get('/transactions');
      set({ transactions: response.data });
    } catch (error) {
      set({ error: 'Erro ao carregar transações' });
    } finally {
      set({ isLoading: false });
    }
  },

  addTransaction: async (transaction) => {
    try {
      set({ isLoading: true, error: null });
      const response = await api.post('/transactions', transaction);
      set((state) => ({
        transactions: [...state.transactions, response.data],
      }));
      await get().updatePortfolio();
    } catch (error) {
      set({ error: 'Erro ao adicionar transação' });
    } finally {
      set({ isLoading: false });
    }
  },

  updatePortfolio: async () => {
    try {
      set({ isLoading: true, error: null });
      const response = await api.get('/portfolio/summary');
      set({
        portfolioValue: response.data.portfolioValue,
        dailyChange: response.data.dailyChange,
        monthlyChange: response.data.monthlyChange,
        totalInvested: response.data.totalInvested,
      });
    } catch (error) {
      set({ error: 'Erro ao atualizar portfólio' });
    } finally {
      set({ isLoading: false });
    }
  },
}));

export default useStore; 