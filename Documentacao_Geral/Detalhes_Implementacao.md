# Detalhes da Implementação dos Testes

## Estrutura do Projeto de Testes

### Dependências
```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
<PackageReference Include="Moq" Version="4.20.70" />
<PackageReference Include="xunit" Version="2.6.6" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
<PackageReference Include="coverlet.collector" Version="6.0.0" />
```

### Organização dos Testes
Os testes estão organizados em uma única classe `InvestServiceTests`, que contém:
- Mocks das dependências (`IRepository` e `ICotacaoService`)
- Setup inicial no construtor
- Testes agrupados por funcionalidade

## Padrões Utilizados

### 1. Padrão AAA (Arrange, Act, Assert)
Todos os testes seguem o padrão AAA:
```csharp
// Arrange
var usuarioId = 1;
var usuarioEsperado = new Usuario { Id = usuarioId, Nome = "Teste" };
_mockRepository.Setup(r => r.GetUsuarioAsync(usuarioId))
    .ReturnsAsync(usuarioEsperado);

// Act
var resultado = await _investService.GetUsuarioAsync(usuarioId);

// Assert
Assert.NotNull(resultado);
Assert.Equal(usuarioId, resultado.Id);
```

### 2. Nomenclatura dos Testes
Os testes seguem o padrão:
`[Metodo]_[Cenario]_[ResultadoEsperado]`

Exemplo:
```csharp
GetUsuarioAsync_QuandoUsuarioExiste_RetornaUsuario
```

### 3. Uso de Mocks
Utilizamos o Moq para simular dependências:
```csharp
_mockRepository.Setup(r => r.GetUsuarioAsync(usuarioId))
    .ReturnsAsync(usuarioEsperado);
```

## Casos de Teste Detalhados

### 1. Testes de Usuário
```csharp
[Fact]
public async Task GetUsuarioAsync_QuandoUsuarioExiste_RetornaUsuario()
{
    // Arrange
    var usuarioId = 1;
    var usuarioEsperado = new Usuario { Id = usuarioId, Nome = "Teste" };
    _mockRepository.Setup(r => r.GetUsuarioAsync(usuarioId))
        .ReturnsAsync(usuarioEsperado);

    // Act
    var resultado = await _investService.GetUsuarioAsync(usuarioId);

    // Assert
    Assert.NotNull(resultado);
    Assert.Equal(usuarioId, resultado.Id);
    Assert.Equal("Teste", resultado.Nome);
}
```

### 2. Testes de PnL
```csharp
[Fact]
public async Task GetPosicaoGlobalComPnlAsync_ComMultiplosAtivos_CalculaValoresCorretamente()
{
    // Arrange
    var posicoes = new List<PosicaoPorPapel>
    {
        new PosicaoPorPapel 
        { 
            AtivoId = 1, 
            Codigo = "PETR4", 
            QuantidadeLiquida = 100,
            PrecoMedio = 25.50m,
            PrecoAtual = 26.00m
        },
        new PosicaoPorPapel 
        { 
            AtivoId = 2, 
            Codigo = "VALE3", 
            QuantidadeLiquida = 200,
            PrecoMedio = 68.75m,
            PrecoAtual = 70.00m
        }
    };

    // Setup dos mocks...

    // Act
    var resultado = await _investService.GetPosicaoGlobalComPnlAsync(usuarioId);

    // Assert
    Assert.Equal(17200m, resultado.ValorMercado);
    Assert.Equal(16300m, resultado.CustoTotal);
    Assert.Equal(900m, resultado.PnL);
}
```

## Boas Práticas Implementadas

1. **Isolamento**
   - Uso de mocks para simular dependências
   - Testes independentes entre si
   - Setup limpo para cada teste

2. **Legibilidade**
   - Nomes descritivos
   - Comentários explicativos
   - Organização clara do código

3. **Manutenibilidade**
   - Código DRY (Don't Repeat Yourself)
   - Setup reutilizável
   - Fácil de estender

4. **Cobertura**
   - Testes positivos e negativos
   - Casos de borda
   - Verificações de comportamento

## Sugestões de Melhorias

1. **Cobertura Adicional**
   - Adicionar testes para exceções
   - Testar mais casos de erro
   - Aumentar cobertura de código

2. **Organização**
   - Separar testes em classes por funcionalidade
   - Criar classes base para setup comum
   - Implementar testes de integração

3. **Documentação**
   - Adicionar mais comentários
   - Documentar casos de uso
   - Criar guia de manutenção 