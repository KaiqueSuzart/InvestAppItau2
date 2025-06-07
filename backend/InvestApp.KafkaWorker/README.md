# InvestApp.KafkaWorker

Este projeto é responsável por consumir e processar mensagens do Apache Kafka relacionadas a eventos do sistema de investimentos. Ele executa tarefas assíncronas, como atualização de cotações, processamento de operações e integração com outros serviços.

## Principais responsabilidades
- Consumir tópicos Kafka para eventos financeiros
- Processar e persistir dados recebidos
- Integrar com o restante do backend para manter os dados atualizados

Este worker é fundamental para garantir o processamento em tempo real e a escalabilidade do sistema.

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