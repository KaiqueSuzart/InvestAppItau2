# InvestApp.KafkaWorker

Este Worker Service .NET consome cotações de um tópico Kafka e salva no banco de dados MySQL, garantindo retry e idempotência.

## Configuração

Edite o arquivo `appsettings.json` com as informações do seu ambiente:

```
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "Topic": "cotacoes"
  },
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=investdb;user=root;password=SUASENHA"
  }
}
```

- **Kafka:BootstrapServers**: Endereço do broker Kafka.
- **Kafka:Topic**: Nome do tópico de cotações.
- **DefaultConnection**: String de conexão com o MySQL.

## Funcionamento

- O Worker fica escutando o tópico Kafka definido.
- Ao receber uma mensagem (cotação), verifica se já existe cotação para o mesmo ativo e data/hora (idempotência).
- Se não existir, insere no banco de dados.
- Em caso de erro, faz retry automático com delay.

## Execução

1. Instale as dependências:
   - Confluent.Kafka
   - Dapper
   - MySql.Data
2. Configure o `appsettings.json`.
3. Execute o Worker:
   ```sh
   dotnet run --project InvestApp.KafkaWorker
   ```

## Exemplo de mensagem Kafka
```json
{
  "AtivoId": 1,
  "PrecoUnitario": 25.50,
  "DataHora": "2024-06-07T15:00:00"
}
```

## Observações
- O Worker é resiliente a falhas e garante que não haverá duplicidade de cotações.
- Ajuste o nome do tópico e a string de conexão conforme seu ambiente. 