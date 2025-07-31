# Configurações de Mapeamento Entity Framework Core

Este projeto utiliza `IEntityTypeConfiguration` para configurar o mapeamento dos models para o banco de dados SQL.

## Estrutura

### Models
- `Servico`: Representa os serviços oferecidos
- `Agendamento`: Representa os agendamentos de clientes

### Configurações
- `ServicoConfiguration`: Configuração de mapeamento para a tabela Servicos
- `AgendamentoConfiguration`: Configuração de mapeamento para a tabela Agendamentos

## Características das Configurações

### ServicoConfiguration
- Tabela: `Servicos`
- Chave primária: `Id` (auto-incremento)
- Campos:
  - `Nome`: VARCHAR(100), obrigatório
  - `Descricao`: VARCHAR(500), opcional
  - `Preco`: DECIMAL(18,2), obrigatório
  - `DuracaoEmMinutos`: INT, obrigatório
- Índices: `Nome`
- Relacionamento: Um-para-muitos com Agendamentos

### AgendamentoConfiguration
- Tabela: `Agendamentos`
- Chave primária: `Id` (auto-incremento)
- Campos:
  - `DataHora`: DATETIME2, obrigatório
  - `NomeDoCliente`: VARCHAR(100), obrigatório
  - `NumeroDoCliente`: VARCHAR(20), obrigatório
  - `ServicoId`: INT, obrigatório (chave estrangeira)
  - `Observacoes`: VARCHAR(1000), opcional
- Índices: `DataHora`, `NomeDoCliente`, `ServicoId`
- Relacionamento: Muitos-para-um com Servicos
- Constraints:
  - DataHora deve ser futura
  - ServicoId deve referenciar um serviço com preço positivo

## Como Aplicar as Configurações

As configurações são aplicadas automaticamente no `ApplicationDbContext` através do método `OnModelCreating`.

## Gerando Migration

Para aplicar as configurações ao banco de dados, execute:

```bash
dotnet ef migrations add NomeDaMigration
dotnet ef database update
```

## Vantagens do IEntityTypeConfiguration

1. **Separação de Responsabilidades**: Configurações separadas dos models
2. **Reutilização**: Configurações podem ser reutilizadas em diferentes contextos
3. **Manutenibilidade**: Fácil de manter e modificar
4. **Testabilidade**: Configurações podem ser testadas independentemente
5. **Organização**: Código mais organizado e legível 