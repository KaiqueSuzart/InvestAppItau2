# InvestApp.KafkaWorker

Este projeto é responsável por consumir e processar mensagens do Apache Kafka relacionadas a eventos do sistema de investimentos. Ele executa tarefas assíncronas, como atualização de cotações, processamento de operações e integração com outros serviços.

## Principais responsabilidades
- Consumir tópicos Kafka para eventos financeiros
- Processar e persistir dados recebidos
- Integrar com o restante do backend para manter os dados atualizados

Este worker é fundamental para garantir o processamento em tempo real e a escalabilidade do sistema. 