# InvestApp.KafkaWorker

Este é um serviço worker que utiliza Apache Kafka para processamento assíncrono de atualizações de cotações e posições de investimentos.

## Propósito

O KafkaWorker foi desenvolvido para:
- Processar atualizações de cotações em tempo real
- Atualizar posições de investimentos de forma assíncrona
- Reduzir a carga no servidor principal da API
- Garantir processamento confiável de atualizações de preços

## Funcionalidades

- Consome mensagens de cotações do Kafka
- Atualiza o banco de dados com novas cotações
- Recalcula posições dos usuários
- Processa atualizações em background

## Configuração

Para executar o worker:

```bash
cd InvestApp.KafkaWorker
dotnet run
```

## Dependências

- Apache Kafka
- .NET 6.0
- MySQL Database

## Observações

Este worker deve ser executado separadamente da API principal, pois ele é um serviço independente que processa atualizações em background. 