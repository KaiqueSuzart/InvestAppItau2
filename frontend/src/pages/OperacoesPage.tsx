import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate } from "react-router-dom";
import { useInvestData } from "@/hooks/useInvestData";
import api from "@/services/api";
import { Pencil, Trash2, Plus } from 'lucide-react';

const PAGE_SIZE = 10;

function useQuery() {
  return new URLSearchParams(useLocation().search);
}

export default function OperacoesPage() {
  const query = useQuery();
  const navigate = useNavigate();
  const location = useLocation();
  const usuarioIdParam = query.get("usuarioId");
  const usuarioId = usuarioIdParam ? parseInt(usuarioIdParam, 10) : NaN;
  const { posicaoGlobal, loading: loadingGlobal, error } = useInvestData(usuarioId);
  const [loading, setLoading] = useState(false);
  const [nomeUsuario, setNomeUsuario] = React.useState<string>("");
  const [filtroAtivo, setFiltroAtivo] = React.useState("");
  const [filtroTipo, setFiltroTipo] = React.useState("");
  const [filtroDataInicio, setFiltroDataInicio] = React.useState("");
  const [filtroDataFim, setFiltroDataFim] = React.useState("");
  const [operacoes, setOperacoes] = useState([]);
  const [pagina, setPagina] = useState(1);
  const [total, setTotal] = useState(0);
  const [ordenarPor, setOrdenarPor] = useState('data');
  const [ordem, setOrdem] = useState('desc');
  const [autocomplete, setAutocomplete] = useState([]);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showNewModal, setShowNewModal] = useState(false);
  const [editData, setEditData] = useState(null);

  React.useEffect(() => {
    let id = usuarioId;
    if (isNaN(id)) {
      // Tenta recuperar do localStorage
      const storedId = localStorage.getItem('usuarioId');
      if (storedId && !isNaN(Number(storedId))) {
        id = Number(storedId);
        // Redireciona para a mesma página com o usuarioId na URL
        navigate(`${location.pathname}?usuarioId=${id}`, { replace: true });
        return;
      }
      // Se não encontrar, vai para login
      navigate("/");
    } else {
      // Salva no localStorage para persistir entre páginas
      localStorage.setItem('usuarioId', id.toString());
    }
  }, [usuarioId, navigate, location.pathname]);

  // Buscar nome do usuário logado ao carregar
  React.useEffect(() => {
    let id = usuarioId;
    if (!isNaN(id)) {
      api.get(`/invest/usuario/${id}`)
        .then(res => {
          console.log('Resposta usuário:', res.data); // Log para depuração
          if (res.data && res.data.nome) {
            setNomeUsuario(res.data.nome);
            localStorage.setItem('nomeUsuario', res.data.nome);
          }
        })
        .catch(() => setNomeUsuario(""));
    } else {
      // Se não tem id, tenta pegar do localStorage
      const nomeSalvo = localStorage.getItem('nomeUsuario');
      if (nomeSalvo) setNomeUsuario(nomeSalvo);
    }
  }, [usuarioId]);

  useEffect(() => {
    fetchOperacoes();
  }, [pagina, ordenarPor, ordem, filtroAtivo]);

  async function fetchOperacoes() {
    setLoading(true);
    const res = await api.get(`/invest/usuario/${usuarioId}/operacoes`, {
      params: {
        pagina,
        tamanho: PAGE_SIZE,
        ordenarPor,
        ordem,
        filtroAtivo: filtroAtivo || undefined
      }
    });
    setOperacoes(res.data.operacoes);
    setTotal(res.data.total);
    setLoading(false);
  }

  async function handleAutocomplete(query) {
    if (!query) return setAutocomplete([]);
    const res = await api.get(`/invest/usuario/${usuarioId}/operacoes`, { params: { filtroAtivo: query, tamanho: 5 } });
    setAutocomplete([...new Set(res.data.operacoes.map(o => o.Ativo || o.ativo))]);
  }

  function handleSort(col) {
    if (ordenarPor === col) setOrdem(ordem === 'asc' ? 'desc' : 'asc');
    else {
      setOrdenarPor(col);
      setOrdem('asc');
    }
    setPagina(1);
  }

  function handleEdit(op) {
    setEditData(op);
    setShowEditModal(true);
  }

  function handleDelete(id) {
    if (window.confirm('Tem certeza que deseja excluir esta operação?')) {
      api.delete(`/invest/operacao/${id}`).then(fetchOperacoes);
    }
  }

  function handleNew() {
    setEditData(null);
    setShowNewModal(true);
  }

  const totalPages = Math.ceil(total / PAGE_SIZE);
  const pages = Array.from({ length: totalPages }, (_, i) => i + 1);

  return (
    <div className="min-h-screen bg-[#F5F5F5] flex flex-col items-center">
      <div className="w-full max-w-5xl mt-16">
        <div className="flex items-center justify-between mb-6 operacoes-header">
          <h1 className="text-2xl font-bold text-[#004080]">Operações</h1>
        </div>
        <div className="flex items-center justify-between mb-4">
          <div className="relative w-64">
            <input
              value={filtroAtivo}
              onChange={e => { setFiltroAtivo(e.target.value); handleAutocomplete(e.target.value); }}
              placeholder="Buscar por ativo..."
              className="w-full border rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[#004080]"
            />
            {autocomplete.length > 0 && (
              <ul className="absolute bg-white border w-full z-10 rounded shadow">
                {autocomplete.map((a, i) => (
                  <li key={i} className="px-3 py-2 hover:bg-gray-100 cursor-pointer" onClick={() => { setFiltroAtivo(a); setAutocomplete([]); }}>{a}</li>
                ))}
              </ul>
            )}
          </div>
          <button onClick={handleNew} className="flex items-center gap-2 bg-[#004080] text-white px-4 py-2 rounded font-medium hover:bg-[#003366] transition-colors"><Plus size={18}/> Nova Operação</button>
        </div>
        <div className="overflow-x-auto bg-white rounded-xl shadow w-full">
          <table className="min-w-full text-base operacoes-table">
            <thead>
              <tr>
                {['Data', 'Ativo', 'Tipo', 'Quantidade', 'Preço', 'Valor total', 'Ações'].map(col => (
                  <th
                    key={col}
                    className={`px-4 py-3 select-none text-[#004080] ${['Quantidade','Preço','Valor total','Ações'].includes(col) ? 'text-center' : 'text-left'} cursor-pointer`}
                    onClick={() => col !== 'Ações' && handleSort(col === 'Preço' ? 'preco' : col === 'Valor total' ? 'valorTotal' : col.toLowerCase().replace(' ', ''))}
                  >
                    {col !== 'Ações' && (
                      <span>
                        {col}
                        {ordenarPor === (col === 'Preço' ? 'preco' : col === 'Valor total' ? 'valorTotal' : col.toLowerCase().replace(' ', '')) && (
                          <span className="ml-1">{ordem === 'asc' ? '▲' : '▼'}</span>
                        )}
                      </span>
                    )}
                    {col === 'Ações' && 'Ações'}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr><td colSpan={7} className="text-center py-8">Carregando...</td></tr>
              ) : operacoes.length === 0 ? (
                <tr><td colSpan={7} className="text-center py-8 text-gray-400">Nenhuma operação encontrada</td></tr>
              ) : (
                operacoes.map(op => (
                  <tr key={op.Id || op.id} className="even:bg-gray-100">
                    <td className="px-4 py-3 align-middle">{new Date(op.Data || op.data).toLocaleDateString('pt-BR')}</td>
                    <td className="px-4 py-3 align-middle">{op.Ativo || op.ativo}</td>
                    <td className="px-4 py-3 align-middle">{op.Tipo || op.tipo}</td>
                    <td className="px-4 py-3 align-middle text-center">{op.Quantidade || op.quantidade}</td>
                    <td className="px-4 py-3 align-middle text-center">{Number(op.Preco ?? op.preco).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</td>
                    <td className="px-4 py-3 align-middle text-center">{Number(op.ValorTotal ?? op.valorTotal).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</td>
                    <td className="px-4 py-3 align-middle text-center">
                      <div className="flex gap-2 justify-center items-center operacoes-actions">
                        <button type="button" onClick={() => handleEdit(op)} title="Editar" className="p-1 rounded hover:bg-blue-100 transition-colors"><Pencil size={18} className="text-blue-600 hover:text-blue-800"/></button>
                        <button type="button" onClick={() => handleDelete(op.Id || op.id)} title="Excluir" className="p-1 rounded hover:bg-red-100 transition-colors"><Trash2 size={18} className="text-red-600 hover:text-red-800"/></button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
        <div className="flex justify-center gap-2 mt-6 operacoes-paginacao">
          {pages.map(p => (
            <button
              key={p}
              className={`px-3 py-1 rounded ${p === pagina ? 'bg-[#004080] text-white' : 'bg-gray-200 text-gray-700'} hover:bg-[#FF6600] hover:text-white`}
              onClick={() => setPagina(p)}
            >
              {p}
            </button>
          ))}
          {pagina < totalPages && (
            <button className="px-3 py-1 rounded bg-gray-200 text-gray-700 hover:bg-[#FF6600] hover:text-white" onClick={() => setPagina(pagina + 1)}>Próximo</button>
          )}
        </div>
        {/* Modais de edição e nova operação */}
        {(showEditModal || showNewModal) && (
          <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg shadow-lg p-8 w-full max-w-md relative">
              <h2 className="text-xl font-bold mb-4 text-[#004080]">{showEditModal ? 'Editar Operação' : 'Nova Operação'}</h2>
              <form onSubmit={async (e) => {
                e.preventDefault();
                const form = e.target;
                let data;
                if (showEditModal && editData) {
                  data = {
                    Id: editData?.id || editData?.Id,
                    Quantidade: Number(form.quantidade.value),
                    Preco: Number(form.preco.value),
                    TipoOperacao: form.tipo.value,
                    Data: form.data.value,
                    Ativo: form.ativo.value
                  };
                  await api.put(`/invest/operacao/${editData.Id || editData.id}`, data);
                } else {
                  data = {
                    Quantidade: Number(form.quantidade.value),
                    Preco: Number(form.preco.value),
                    TipoOperacao: form.tipo.value,
                    Data: form.data.value,
                    Ativo: form.ativo.value
                  };
                  await api.post(`/invest/usuario/${usuarioId}/operacao`, data);
                }
                setShowEditModal(false);
                setShowNewModal(false);
                setEditData(null);
                fetchOperacoes();
              }}>
                <div className="mb-3">
                  <label className="block text-sm font-medium text-gray-700 mb-1">Ativo</label>
                  <input name="ativo" defaultValue={editData?.Ativo || editData?.ativo || ''} required className="w-full border rounded px-3 py-2" />
                </div>
                <div className="mb-3">
                  <label className="block text-sm font-medium text-gray-700 mb-1">Tipo</label>
                  <select name="tipo" defaultValue={editData?.Tipo || editData?.tipo || 'COMPRA'} required className="w-full border rounded px-3 py-2">
                    <option value="COMPRA">COMPRA</option>
                    <option value="VENDA">VENDA</option>
                  </select>
                </div>
                <div className="mb-3">
                  <label className="block text-sm font-medium text-gray-700 mb-1">Quantidade</label>
                  <input name="quantidade" type="number" min="1" defaultValue={editData?.Quantidade || editData?.quantidade || ''} required className="w-full border rounded px-3 py-2" />
                </div>
                <div className="mb-3">
                  <label className="block text-sm font-medium text-gray-700 mb-1">Preço</label>
                  <input name="preco" type="number" step="0.01" min="0" defaultValue={editData?.Preco || editData?.preco || ''} required className="w-full border rounded px-3 py-2" />
                </div>
                <div className="mb-3">
                  <label className="block text-sm font-medium text-gray-700 mb-1">Data</label>
                  <input name="data" type="date" defaultValue={editData?.Data ? editData.Data.slice(0,10) : (editData?.data ? editData.data.slice(0,10) : '')} required className="w-full border rounded px-3 py-2" />
                </div>
                <div className="flex justify-end gap-2 mt-6">
                  <button type="button" onClick={() => { setShowEditModal(false); setShowNewModal(false); setEditData(null); }} className="px-4 py-2 rounded bg-gray-200 text-gray-700 hover:bg-gray-300">Cancelar</button>
                  <button type="submit" className="px-4 py-2 rounded bg-[#004080] text-white hover:bg-[#003366]">Salvar</button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </div>
  );
} 