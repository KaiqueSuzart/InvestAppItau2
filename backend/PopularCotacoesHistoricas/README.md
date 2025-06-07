# PopularCotacoesHistoricas

Este projeto é responsável por popular o banco de dados com cotações históricas de ativos financeiros. Ele pode ser executado para importar dados de fontes externas e garantir que o sistema tenha um histórico completo de preços.

## Principais responsabilidades
- Importar cotações históricas de ativos
- Preencher o banco de dados para análises e gráficos
- Automatizar a atualização de dados históricos

É utilizado principalmente em fases de setup inicial ou atualização de grandes volumes de dados.

## Propósito

O projeto foi desenvolvido para:
- Importar dados históricos de cotações de ativos
- Popular o banco de dados com informações passadas
- Facilitar testes e desenvolvimento com dados reais
- Permitir análise de performance histórica

## Funcionalidades

- Importa cotações históricas de fontes externas
- Processa e formata dados de acordo com o modelo do banco
- Insere cotações no banco de dados de forma otimizada
- Suporta diferentes períodos e ativos

## Como Usar

Para executar o importador:

```bash
cd PopularCotacoesHistoricas
dotnet run
```

## Configuração

O projeto utiliza:
- Arquivo de configuração para definir ativos e períodos
- Conexão com o mesmo banco de dados do InvestApp
- Processamento em lotes para melhor performance

## Observações

- Este é um projeto utilitário, não parte do sistema principal
- Deve ser executado apenas quando necessário popular dados históricos
- Não afeta o funcionamento normal do InvestApp 