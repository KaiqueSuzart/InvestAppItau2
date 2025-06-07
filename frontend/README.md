# InvestApp

## Backend
```bash
cd investapp/backend
dotnet restore
dotnet run
# Backend will run on http://localhost:5000
```

## Frontend
```bash
cd investapp/frontend
npm install
npm run dev
# Frontend will run on http://localhost:5173 and proxy `/api` to http://localhost:5000
```

## Configuração

- O backend está configurado com CORS para permitir requisições de `http://localhost:5173`
- O frontend está configurado para fazer proxy de `/api` para `http://localhost:5000`
- Para acessar a aplicação, abra `http://localhost:5173` no navegador
