# Desafio Técnico Itaú – InvestApp

## Contexto
Renda variável é uma modalidade de investimento em que os retornos não são fixos ou previsíveis, como ocorre na renda fixa. A principal característica desse tipo de investimento é a possibilidade de ganhos (ou perdas) conforme as oscilações do mercado. Exemplos: ações, FIIs, ETFs. Este projeto simula um sistema completo de controle de investimentos, com foco em performance, resiliência, testes e documentação.

## Glossário
- **Ativo:** Item negociável no mercado financeiro (ex: ITSA3, KDIF11).Add commentMore actions
- **Preço:** Determinado por oferta e demanda.
- **Bolsa de Valores:** Ambiente de negociação (ex: B3).
- **Volatilidade:** Variação dos preços de um ativo.
- **Dividendos/JSP:** Lucro distribuído aos acionistas/cotistas.
- **Preço Médio:** Média ponderada dos preços das operações, descontando dividendos.
- **P&L:** Lucro/Prejuízo do cliente (Profit & Loss).
- **Tipo Operação:** Compra ou Venda.
- **Corretagem:** Valor pago à corretora por intermediar operações.

  ## O que foi desenvolvido
Este repositório contém uma solução completa para o desafio, cobrindo todos os tópicos propostos:

### 1. Modelagem de Banco Relacional
- Script SQL em `Documentacao_Geral/modelagem_investdb.sql`.
- Justificativa dos tipos de dados em `Documentacao_Geral/Modelagem_BD.txt`.Add commentMore actions

### 2. Índices e Performance
- Proposta de índices e consulta otimizada documentadas.
- Estrutura para atualização de posição baseada em cotação.

### 3. Aplicação
- Backend em .NET (C#) com Dapper e arquitetura limpa.
- Frontend em React + Vite (opcional, para visualização e experiência completa).
- Separação clara de responsabilidades, uso de async/await.

### 4. Lógica de Negócio – Preço Médio
- Método robusto para cálculo do preço médio ponderado, com tratamento de entradas inválidas.

### 5. Testes Unitários
- Testes positivos e negativos com xUnit e Moq.
- Estrutura clara, isolada e de fácil manutenção.

  
### 6. Testes Mutantes
- Explicação do conceito, exemplos práticos e documentação em `Documentacao_Geral/Testes_Mutantes.md`.

### 7. Integração entre Sistemas
- Worker Service .NET para consumir cotações via Kafka, com retry e idempotência.
- Código e configuração em `InvestApp.KafkaWorker/`.

### 8. Engenharia do Caos
- Circuit breaker, fallback e observabilidade implementados com Polly.
- Logs detalhados e fallback para última cotação local.

### 9. Escalabilidade e PerformanceAdd commentMore actions
- Documentação sobre auto-scaling horizontal e balanceamento de carga em `Documentacao_Geral/Escalabilidade_e_Performance.md`.

### 10. Documentação e APIs
- APIs RESTful para todas as operações do desafio.
- Documentação OpenAPI 3.0 em `Documentacao_Geral/OpenAPI_investapp.yaml`.
- Exemplos de endpoints, parâmetros e respostas.

## Estrutura do Projeto
```
InvestApp/Add commentMore actions
│
├── backend/                # Backend .NET (APIs, serviços, controllers)
│   ├── Controllers/        # Controllers REST
│   ├── Services/           # Lógica de negócio, resiliência, integração
│   ├── Models/             # Modelos de dados
│   ├── Data/               # Acesso a banco de dados
│   └── ...
│
├── frontend/               # Frontend React + Vite (opcional)
│   └── ...
│
├── InvestApp.KafkaWorker/  # Worker Service .NET para Kafka
│   ├── Worker.cs           # Consumo Kafka, retry, idempotência
│   └── ...
│
├── Documentacao_Geral/     # Toda a documentação do projeto
│   ├── OpenAPI_investapp.yaml         # Documentação OpenAPI 3.0 dos endpoints
│   ├── Modelagem_BD.txt               # Explicação da modelagem do banco
│   ├── modelagem_investdb.sql         # Script SQL do banco
│   ├── Testes_Mutantes.md             # Explicação e exemplos de testes mutantes
│   ├── Resumo_Testes.md               # Resumo dos testes automatizados
│   ├── Detalhes_Implementacao.md      # Detalhes técnicos e sugestões
│   ├── Resultados_Testes.md           # Resultados e cobertura dos testes
│   ├── Escalabilidade_e_Performance.md# Estratégias de auto-scaling e balanceamento
│   └── README.md                      # (Opcional) Índice da documentação
│
└── README.md               # Este arquivo, visão geral do projeto

  

 ## Como rodar o projeto
1. **Banco de Dados:**
   - Execute o script `Documentacao_Geral/modelagem_investdb.sql` no MySQL.Add commentMore actions
2. **Backend:**
   - Ajuste as strings de conexão em `backend/appsettings.json`.
   - Rode com `dotnet run --project backend`.
3. **Frontend (opcional):**
   - Instale dependências com `npm install`.
   - Rode com `npm run dev`.
4. **Worker Kafka:**
   - Configure Kafka e banco em `InvestApp.KafkaWorker/appsettings.json`.
   - Rode com `dotnet run --project InvestApp.KafkaWorker`.

## Onde está cada documentação
- **Modelagem de banco:** `Documentacao_Geral/modelagem_investdb.sql`, `Documentacao_Geral/Modelagem_BD.txt`
- **Testes e resultados:** `Documentacao_Geral/Resumo_Testes.md`, `Documentacao_Geral/Resultados_Testes.md`
- **Testes mutantes:** `Documentacao_Geral/Testes_Mutantes.md`
- **Escalabilidade e performance:** `Documentacao_Geral/Escalabilidade_e_Performance.md`Add commentMore actions
- **APIs RESTful:** `Documentacao_Geral/OpenAPI_investapp.yaml`
- **Detalhes técnicos:** `Documentacao_Geral/Detalhes_Implementacao.md`


## Observações Finais
- O projeto foi desenvolvido com foco em clareza, qualidade, criatividade e performance, conforme orientações do desafio.
- Todos os pontos do enunciado foram implementados e documentados.
- O uso de IA foi feito para acelerar e garantir qualidade, sempre com revisão crítica e adaptação ao contexto do Itaú.


**Obrigado pela oportunidade!**

> _Autor: Kaique Suzart_ 
