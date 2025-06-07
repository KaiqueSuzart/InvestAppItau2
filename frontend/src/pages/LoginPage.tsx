import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { motion } from "framer-motion";
import api from "@/services/api";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!email || !senha) {
      setError("Preencha o e-mail e a senha.");
      return;
    }
    setError("");
    setLoading(true);
    try {
      const res = await api.post("/auth/login", { email, senha });
      if (res.data && res.data.id) {
        localStorage.setItem('usuarioId', res.data.id);
        navigate(`/dashboard?usuarioId=${res.data.id}`);
      } else if (res.data && res.data.usuario && res.data.usuario.Id) {
        localStorage.setItem('usuarioId', res.data.usuario.Id);
        navigate(`/dashboard?usuarioId=${res.data.usuario.Id}`);
      } else {
        setError("Resposta inesperada do servidor.");
      }
    } catch (err: any) {
      setError(err.response?.data || "Usuário ou senha inválidos.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="bg-[#F5F5F5] min-h-screen flex items-center justify-center">
      <motion.div
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        transition={{ duration: 0.5 }}
        className="bg-white rounded-2xl shadow-lg py-8 px-6 w-full max-w-sm"
      >
        <h2 className="text-2xl font-semibold text-[#004080] mb-6 text-center font-inter">Login</h2>
        <form onSubmit={handleSubmit}>
          <input
            type="email"
            placeholder="E-mail"
            value={email}
            onChange={e => setEmail(e.target.value)}
            className={`w-full p-3 border rounded-md focus:outline-none focus:ring-2 focus:ring-[#004080] mb-4 ${error ? "border-red-500" : "border-gray-300"}`}
            autoComplete="username"
          />
          <input
            type="password"
            placeholder="Senha"
            value={senha}
            onChange={e => setSenha(e.target.value)}
            className={`w-full p-3 border rounded-md focus:outline-none focus:ring-2 focus:ring-[#004080] mb-4 ${error ? "border-red-500" : "border-gray-300"}`}
            autoComplete="current-password"
          />
          {error && <div className="text-red-600 text-sm mb-2">{error}</div>}
          <button
            type="submit"
            className="w-full bg-[#FF6600] hover:bg-[#e65c00] text-white py-3 rounded-md font-semibold transition-colors duration-200"
            disabled={loading}
          >
            {loading ? "Entrando..." : "Entrar"}
          </button>
        </form>
      </motion.div>
    </div>
  );
} 