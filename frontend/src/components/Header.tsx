import { NavLink, useLocation } from "react-router-dom";
import React from "react";
import api from "../services/api";

export function Header() {
  const location = useLocation();
  const [nomeUsuario, setNomeUsuario] = React.useState<string>("");
  const [titulo, setTitulo] = React.useState<string>("");

  React.useEffect(() => {
    // Pega usuarioId da query string ou localStorage
    const params = new URLSearchParams(location.search);
    let usuarioId = params.get("usuarioId") || localStorage.getItem("usuarioId");
    if (usuarioId) {
      api.get(`/invest/usuario/${usuarioId}`)
        .then(res => {
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
  }, [location.search]);

  React.useEffect(() => {
    // Define o título da página pelo path
    if (location.pathname.includes("dashboard")) setTitulo("Dashboard");
    else if (location.pathname.includes("operacoes")) setTitulo("Operações");
    else if (location.pathname.includes("posicoes")) setTitulo("Posições");
    else if (location.pathname.includes("cotacoes")) setTitulo("Cotações");
    else setTitulo("");
  }, [location.pathname]);

  const usuarioId = new URLSearchParams(location.search).get("usuarioId") || localStorage.getItem("usuarioId") || "";

  return (
    <header className="fixed top-0 left-0 w-full h-14 bg-[#004080] flex items-center px-6 shadow-lg z-20 justify-between">
      <div className="flex items-center gap-3">
        <img src="/itau.jpg" alt="Logo Itaú" className="h-9 w-9 rounded-md bg-white p-1" />
        <span className="text-xl font-bold text-white font-inter mr-6">{titulo}</span>
        <nav className="flex gap-6">
          <NavLink
            to={`/dashboard?usuarioId=${usuarioId}`}
            className={({ isActive }) =>
              isActive
                ? "text-white underline underline-offset-4"
                : "text-white/80 hover:text-white hover:scale-105 transition-transform duration-200"
            }
          >
            Dashboard
          </NavLink>
          <NavLink
            to={`/operacoes?usuarioId=${usuarioId}`}
            className={({ isActive }) =>
              isActive
                ? "text-white underline underline-offset-4"
                : "text-white/80 hover:text-white hover:scale-105 transition-transform duration-200"
            }
          >
            Operações
          </NavLink>
          <NavLink
            to={`/posicoes?usuarioId=${usuarioId}`}
            className={({ isActive }) =>
              isActive
                ? "text-white underline underline-offset-4"
                : "text-white/80 hover:text-white hover:scale-105 transition-transform duration-200"
            }
          >
            Posições
          </NavLink>
          <NavLink
            to={`/cotacoes?usuarioId=${usuarioId}`}
            className={({ isActive }) =>
              isActive
                ? "text-white underline underline-offset-4"
                : "text-white/80 hover:text-white hover:scale-105 transition-transform duration-200"
            }
          >
            Cotações
          </NavLink>
        </nav>
      </div>
      <div className="flex items-center gap-3">
        <span className="text-white text-base font-medium">{nomeUsuario || "Usuário"}</span>
        <div className="bg-white rounded-full w-8 h-8 flex items-center justify-center">
          <svg width="24" height="24" fill="#004080"><circle cx="12" cy="8" r="4"/><ellipse cx="12" cy="18" rx="7" ry="4"/></svg>
        </div>
      </div>
    </header>
  );
} 