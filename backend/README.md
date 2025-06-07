# InvestApp

Aplicação console para análise de investimentos desenvolvida em .NET Core com Dapper.

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

3. Execute a aplicação:
```bash
dotnet run
```

A aplicação usa por padrão a string de conexão:
```
Server=localhost;Database=investdb;User Id=KaiqueSuzart;Password=SuaSenhaSegura;
```

Para usar uma string de conexão diferente, defina a variável de ambiente `INVESTDB_CONNECTION`.

## Endpoints da API

### Cálculo de Preço Médio

**Endpoint:** `POST /api/PrecoMedio/calcular`

Calcula o preço médio ponderado das operações de compra.

**Request Body:**
```json
[
  {
    "quantidade": 100,
    "precoUnitario": 10.00,
    "tipoOperacao": "COMPRA"
  },
  {
    "quantidade": 200,
    "precoUnitario": 15.00,
    "tipoOperacao": "COMPRA"
  }
]
```

**Response (200 OK):**
```json
16.67
```

**Response (400 Bad Request):**
```json
{
  "message": "A quantidade deve ser maior que zero."
}
```

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