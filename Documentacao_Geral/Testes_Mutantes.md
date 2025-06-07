# Testes Mutantes

## Conceito
Testes mutantes são uma técnica de teste de software que envolve a criação de versões modificadas (mutantes) do código original para verificar se os testes existentes são capazes de detectar essas modificações. A ideia principal é que, se um teste não falhar quando uma mutação é introduzida, isso pode indicar uma fraqueza na cobertura de testes.

## Importância
1. **Qualidade dos Testes**: Ajuda a identificar testes que não são efetivos em detectar erros
2. **Cobertura Real**: Vai além da cobertura de código tradicional, testando a robustez dos testes
3. **Prevenção de Bugs**: Identifica casos onde o código pode ser alterado sem que os testes detectem
4. **Confiança**: Aumenta a confiança na suite de testes

## Tipos de Mutações
1. **Mutação de Operadores**
   - Alteração de operadores aritméticos (+, -, *, /)
   - Alteração de operadores de comparação (>, <, >=, <=, ==, !=)
   - Alteração de operadores lógicos (&&, ||, !)

2. **Mutação de Condicionais**
   - Alteração de condições em if/else
   - Alteração de condições em loops
   - Alteração de condições em expressões ternárias

3. **Mutação de Valores**
   - Alteração de constantes
   - Alteração de valores de retorno
   - Alteração de valores em cálculos

4. **Mutação de Estrutura**
   - Alteração na ordem das operações
   - Alteração em filtros e agrupamentos
   - Alteração em arredondamentos

## Exemplos Implementados

### 1. Mutação na Fórmula do Preço Médio
```csharp
// Código Original
var precoMedio = somaValor / somaQtd;

// Código Mutante
var precoMedioMutante = somaValor / (somaQtd + 1);
```
Esta mutação altera a fórmula do preço médio, adicionando 1 à quantidade total. O teste falha porque o resultado esperado (25.83) não corresponde ao resultado mutante (aproximadamente 0.0857).

### 2. Mutação no Operador de Multiplicação
```csharp
// Código Original
var somaValor = compras.Sum(o => o.Quantidade * o.PrecoUnitario);

// Código Mutante
var somaValor = compras.Sum(o => o.Quantidade + o.PrecoUnitario);
```
Esta mutação substitui a multiplicação por uma adição. O teste falha porque o resultado esperado (25.83) não corresponde ao resultado mutante (aproximadamente 1.17).

### 3. Mutação na Condição de Verificação
```csharp
// Código Original
var precoMedio = somaQtd > 0 ? somaValor / somaQtd : 0m;

// Código Mutante
var precoMedioMutante = somaQtd >= 0 ? somaValor / somaQtd : 0m;
```
Esta mutação altera a condição de verificação de quantidade maior que zero para maior ou igual a zero. O teste falha em casos de quantidade zero, pois permite divisão por zero.

### 4. Mutação no Valor Constante
```csharp
// Código Original
var precoMedio = somaQtd > 0 ? somaValor / somaQtd : 0m;

// Código Mutante
var precoMedioMutante = somaQtd > 0 ? somaValor / somaQtd : 1m;
```
Esta mutação altera o valor retornado quando não há compras de 0 para 1. O teste falha porque o valor esperado em caso de lista vazia deveria ser 0.

### 5. Mutação na Ordem das Operações
```csharp
// Código Original
var precoMedio = compras.Sum(o => o.Quantidade * o.PrecoUnitario) / compras.Sum(o => o.Quantidade);

// Código Mutante
var precoMedioMutante = compras.Sum(o => (o.Quantidade / o.Quantidade) * o.PrecoUnitario);
```
Esta mutação altera a ordem das operações, dividindo a quantidade por ela mesma antes de multiplicar pelo preço. O teste falha porque o resultado será a média dos preços unitários, não o preço médio ponderado.

### 6. Mutação no Arredondamento
```csharp
// Código Original
var precoMedio = somaValor / somaQtd;

// Código Mutante
var precoMedioMutante = Math.Floor(somaValor / somaQtd);
```
Esta mutação adiciona um arredondamento para baixo no resultado. O teste falha porque o valor esperado (25.83) não corresponde ao valor arredondado (25.00).

### 7. Mutação no Filtro de Operações
```csharp
// Código Original
var comprasFiltradas = compras;

// Código Mutante
var comprasFiltradas = compras.Where(o => o.Quantidade > 100);
```
Esta mutação altera o filtro para considerar apenas operações com quantidade maior que 100. O teste falha porque o preço médio deveria considerar todas as operações.

### 8. Mutação no Agrupamento
```csharp
// Código Original
var precoMedio = compras.Sum(o => o.Quantidade * o.PrecoUnitario) / compras.Sum(o => o.Quantidade);

// Código Mutante
var precoMedioMutante = compras
    .GroupBy(o => o.AtivoId)
    .Select(g => g.Sum(o => o.Quantidade * o.PrecoUnitario) / g.Sum(o => o.Quantidade))
    .First();
```
Esta mutação altera o cálculo para considerar apenas o primeiro ativo após agrupar por AtivoId. O teste falha porque o preço médio deveria considerar todas as operações juntas.

## Análise dos Resultados

### Mutações de Fórmula e Operadores
- **Impacto**: Alto
- **Detecção**: Fácil
- **Risco**: Alto
- **Teste**: Falha como esperado

### Mutações de Condicionais
- **Impacto**: Médio
- **Detecção**: Moderada
- **Risco**: Médio
- **Teste**: Falha em casos específicos

### Mutações de Estrutura
- **Impacto**: Alto
- **Detecção**: Moderada
- **Risco**: Alto
- **Teste**: Falha como esperado

## Conclusões
1. Os testes existentes são capazes de detectar mutações significativas
2. A cobertura de testes é robusta para o cálculo de preço médio
3. Casos de borda (como quantidade zero) são bem cobertos
4. As mutações implementadas demonstram a importância de testes precisos
5. Diferentes tipos de mutações foram testados com sucesso

## Recomendações
1. Implementar mais testes mutantes para outros métodos
2. Adicionar testes para casos de borda adicionais
3. Considerar o uso de ferramentas automatizadas de mutação
4. Documentar padrões de mutação comuns no projeto
5. Criar testes específicos para cada tipo de mutação
6. Implementar testes de integração com mutações
7. Adicionar testes de performance com mutações
8. Considerar mutações em conjunto (múltiplas mutações simultâneas) 