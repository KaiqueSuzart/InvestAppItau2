# InvestApp.UnitTests

Este projeto contém os testes unitários automatizados do backend do InvestApp. Ele garante que as funções, serviços e componentes do sistema funcionem corretamente de forma isolada.

## Principais responsabilidades
- Testar métodos e classes individualmente
- Garantir a qualidade e robustez do código
- Facilitar refatorações seguras

Os testes unitários são executados automaticamente em pipelines de CI/CD e durante o desenvolvimento.

## Propósito

Os testes unitários foram desenvolvidos para:
- Garantir o correto funcionamento de cada componente isoladamente
- Facilitar a manutenção do código
- Prevenir regressões durante o desenvolvimento
- Documentar o comportamento esperado do sistema

## Estrutura dos Testes

Os testes estão organizados por camada:
- `Services/`: Testes dos serviços de negócio
- `Controllers/`: Testes dos endpoints da API
- `Data/`: Testes da camada de acesso a dados
- `Models/`: Testes das classes de domínio

## Executando os Testes

Para executar todos os testes:

```bash
cd InvestApp.UnitTests
dotnet test
```

Para executar testes específicos:

```bash
dotnet test --filter "FullyQualifiedName~NomeDoTeste"
```

## Cobertura de Testes

O projeto utiliza:
- xUnit como framework de testes
- Moq para mocks
- FluentAssertions para assertions mais legíveis

## Boas Práticas

- Cada teste segue o padrão AAA (Arrange, Act, Assert)
- Testes são independentes entre si
- Uso de mocks para isolar dependências
- Nomes descritivos que explicam o cenário testado 