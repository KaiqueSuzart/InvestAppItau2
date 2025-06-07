import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { motion } from "framer-motion";

export default function LoginPage() {
  const [userId, setUserId] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!userId || isNaN(Number(userId)) || Number(userId) <= 0) {
      setError("Digite um ID válido.");
      return;
    }
    setError("");
    localStorage.setItem('usuarioId', userId);
    navigate(`/dashboard?usuarioId=${userId}`);
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
            type="number"
            placeholder="ID do Usuário"
            value={userId}
            onChange={e => setUserId(e.target.value)}
            className={`w-full p-3 border rounded-md focus:outline-none focus:ring-2 focus:ring-[#004080] mb-4 ${error ? "border-red-500" : "border-gray-300"}`}
          />
          {error && <div className="text-red-600 text-sm mb-2">{error}</div>}
          <button
            type="submit"
            className="w-full bg-[#FF6600] hover:bg-[#e65c00] text-white py-3 rounded-md font-semibold transition-colors duration-200"
          >
            Entrar
          </button>
        </form>
      </motion.div>
    </div>
  );
} 