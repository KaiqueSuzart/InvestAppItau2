# InvestApp - Backend

API para análise de investimentos desenvolvida em .NET Core com Dapper.

## Pré-requisitos

- MySQL Server 8.0 ou superior
- .NET 6.0 SDK ou superior
- Git

## Configuração do Banco de Dados

1. Execute o script de criação do banco de dados:
   ```bash
   mysql -u root -p < schema.sql
   ```
2. Crie o usuário KaiqueSuzart no MySQL:
   ```sql
   CREATE USER 'KaiqueSuzart'@'localhost' IDENTIFIED BY 'Al101299*';
   GRANT ALL PRIVILEGES ON investdb.* TO 'KaiqueSuzart'@'localhost';
   FLUSH PRIVILEGES;
   ```

## Compilação e Execução

1. Restaure as dependências:
   ```bash
   dotnet restore
   ```
2. Compile o projeto:
   ```bash
   dotnet build
   ```
3. Execute a aplicação (na pasta backend):
   ```bash
   dotnet run
   ```

A aplicação usa por padrão a string de conexão:
```
Server=localhost;Database=investdb;User Id=KaiqueSuzart;Password=SuaSenhaSegura;
```
Para usar uma string de conexão diferente, defina a variável de ambiente `INVESTDB_CONNECTION`.

---

## Endpoints da API (REST)

### AuthController (`/auth`)
- **POST `/auth/login`**: Login de usuário (envie `email` e `senha` no corpo). Retorna o `Id` do usuário se autenticado.
- **GET `/auth/usuarios`**: Lista todos os usuários cadastrados (id e nome).

### InvestController (`/invest`)
- **GET `/invest/usuario/{usuarioId}`**: Retorna nome do usuário.
- **GET `/invest/usuario/{usuarioId}/totalInvestido`**: Lista o total investido por ativo do usuário.
- **GET `/invest/usuario/{usuarioId}/posicaoPorPapel`**: Lista a posição detalhada por papel (ativo) do usuário.
- **GET `/invest/usuario/{usuarioId}/posicaoGlobal`**: Retorna posição global consolidada do usuário (valor de mercado, custo total, PnL).
- **GET `/invest/usuario/{usuarioId}/totalCorretagem`**: Retorna o total de corretagem pago pelo usuário.
- **POST `/invest/usuario/{usuarioId}/atualizarPosicoes`**: Atualiza manualmente as posições do usuário.
- **GET `/invest/usuario/{usuarioId}/historicoPortfolio`**: Retorna o histórico de valor da carteira do usuário ao longo do tempo.
- **GET `/invest/ativo/{ativoId}/cotacoes`**: Lista todas as cotações de um ativo.
- **POST `/invest/cotacao/inserir`**: Insere uma nova cotação para um ativo.
- **GET `/invest/usuario/{usuarioId}/operacoes`**: Lista as operações do usuário (com paginação, ordenação e filtro por ativo).
- **POST `/invest/usuario/{usuarioId}/operacao`**: Cria uma nova operação para o usuário.
- **PUT `/invest/operacao/{id}`**: Edita uma operação existente.

### PrecoMedioController (`/api/PrecoMedio`)
- **POST `/api/PrecoMedio/calcular`**: Calcula o preço médio ponderado de uma lista de operações de compra (envie lista de operações no corpo).

---

## Estrutura do Projeto

- `Models/`: Classes de domínio
- `Data/`: Camada de acesso a dados com Dapper
- `Services/`: Lógica de negócios
- `Controllers/`: Endpoints da API
- `Program.cs`: Ponto de entrada da aplicação

## Dados de Teste

O script `schema.sql` inclui:
- Um usuário de teste (ID: 1)
- Dois ativos (ITSA3 e PETR4)
- Quatro operações de teste
- Cotações atuais para os ativos

---

Consulte a documentação e os controllers para detalhes completos de cada endpoint e exemplos de uso.

## Exemplo de Saída

Ao executar o programa com o usuário ID 1, você verá uma saída similar a:

```
Total Investido por Ativo:
Ativo 1: R$ 1.610,00
Ativo 2: R$ 5.007,50

Posição por Papel:
Ativo 1: 120,00 @ R$ 10,67
Ativo 2: 200,00 @ R$ 25,00

Posição Global:
Valor de Mercado: R$ 6.580,00
Custo Total: R$ 6.280,00
Lucro/Prejuízo: R$ 300,00

Total de Corretagem: R$ 22,50
``` 