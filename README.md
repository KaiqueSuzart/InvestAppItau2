# InvestApp

## Estrutura do Repositório

- backend/: projeto .NET 6 com Dapper e MySQL
- frontend/: projeto React + Vite + TypeScript + Tailwind CSS

## Pré‐requisitos

- .NET 6.0 SDK ou superior instalado
- Node.js 16+ e npm instalados
- MySQL Server rodando
- Crie o schema e tabelas executando o arquivo `schema.sql` (localizado em backend/ ou na raiz, conforme for criado).

## Configurar o Banco de Dados

1. Executar no MySQL:
```sql
CREATE DATABASE IF NOT EXISTS investdb;
USE investdb;
-- comando(s) para criar tabelas e inserir dados de teste
```

2. Criar usuário `KaiqueSuzart` e conceder privilégios:
```sql
CREATE USER IF NOT EXISTS 'KaiqueSuzart'@'localhost' IDENTIFIED BY 'SenhaProjeto2025';
GRANT ALL PRIVILEGES ON investdb.* TO 'KaiqueSuzart'@'localhost';
FLUSH PRIVILEGES;
```

## Executar o Backend

```bash
cd InvestApp/backend
dotnet restore
dotnet run
```

O backend rodará em http://localhost:5000.

CORS está habilitado para http://localhost:5173.

## Executar o Frontend

```bash
cd InvestApp/frontend
npm install
npm run dev
```

O Vite rodará em http://localhost:5173.

As requisições para /api/... serão automaticamente proxied para http://localhost:5000/api.

## Testar a Aplicação

1. Abra http://localhost:5173 no navegador.
2. Digite um ID de usuário válido (por ex. 1) e clique em "Entrar".
3. Você será navegádo para /dashboard?usuarioId=<ID>.

O dashboard irá buscar e exibir:
- Total Investido por Ativo
- Posição por Papel
- Posição Global (Valor de Mercado, Custo Total, P&L)
- Total de Corretagem

## Estrutura de Pastas Resumida

```
InvestApp/
├─ backend/
│   ├─ Controllers/
│   ├─ Data/
│   ├─ Models/
│   ├─ Services/
│   ├─ InvestApp.csproj
│   └─ Program.cs
├─ frontend/
│   ├─ public/
│   │   └─ index.html
│   ├─ src/
│   │   ├─ assets/
│   │   ├─ components/
│   │   ├─ hooks/
│   │   ├─ pages/
│   │   ├─ services/
│   │   │   └─ api.ts
│   │   ├─ styles/
│   │   │   └─ index.css
│   │   ├─ App.tsx
│   │   └─ main.tsx
│   ├─ package.json
│   ├─ tsconfig.json
│   ├─ vite.config.ts
│   └─ tailwind.config.js
├─ README.md
└─ .gitignore
```

## Novidades Técnicas

- Correção do cálculo do valor da carteira ao longo do tempo: agora o último ponto do gráfico reflete exatamente o valor da carteira atual, utilizando as cotações mais recentes disponíveis para cada ativo.
- Implementação de preenchimento automático de cotações históricas para todos os dias úteis, utilizando a última cotação conhecida para cada ativo, garantindo precisão no histórico da carteira.
- Ajustes na API para garantir consistência entre o valor exibido nos cards de totais e o valor apresentado no gráfico de evolução da carteira.
- Integração com a API Alpha Vantage para obtenção de cotações de ativos em tempo real.
- Melhoria na estrutura do backend, eliminando duplicidade de classes e métodos, tornando o código mais limpo e eficiente.
- Garantia de atualização dos dados em tempo real no frontend, refletindo imediatamente as operações e cotações mais recentes. 