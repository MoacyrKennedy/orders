# Sistema de Pedidos (Sales Orders)

## 1. Introdução

O **Sistema de Pedidos (Sales Orders)** é uma aplicação desenvolvida em **.NET** para gerenciamento de pedidos de vendas.

O sistema permite:

* Criação de pedidos
* Atualização de pedidos
* Gerenciamento de clientes, empresas e itens
* Controle de status de pedidos

A aplicação segue princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**, adotando boas práticas de engenharia de software para garantir **manutenibilidade, escalabilidade e testabilidade**.

---

# 2. Modelo Arquitetural

A aplicação utiliza uma arquitetura baseada em **Clean Architecture** combinada com **DDD**, organizada em múltiplas camadas.

## 2.1 Camadas Principais

### Domain (`Sales.Orders.Domain`)

Contém as **regras de negócio centrais do sistema**.

Essa camada é **independente de frameworks externos** e representa o núcleo da aplicação.

#### Entidades

* `Order`
* `OrderItem`
* `BaseEntity`

#### Value Objects

* `Company`
* `Customer`
* `Product`

#### Enums

* `OrderStatus`

  * `Pending`
  * `Processing`
  * `Completed`
  * `Cancelled`

#### Interfaces

* `IOrderRepository`
* `IUnitOfWork`

#### Outros Componentes

* Guards para validações de domínio

---

### Application (`Sales.Orders.Application`)

Contém a **lógica de aplicação** utilizando **CQRS**.

#### Commands

* `CreateOrderCommand`
* `UpdateOrderHeaderCommand`

#### Handlers

Responsáveis por processar os comandos utilizando **MediatR**.

#### Result Pattern

Utilizado para retornar resultados consistentes de operações:

* Sucesso
* Falha
* Erros de validação

---

### Infrastructure (`Sales.Orders.Infrastructure`)

Implementa as **interfaces definidas no domínio**.

Exemplos:

* Implementação de repositórios
* Acesso ao banco de dados
* Integrações externas

---

### API (`Sales.Orders.Api`)

Camada de **apresentação** da aplicação.

Responsável por:

* Expor **endpoints REST**
* Receber requisições
* Encaminhar comandos para o **MediatR**

Tecnologia utilizada:

* **ASP.NET Core**

---

### Tests (`Sales.Orders.Test`)

Contém:

* Testes unitários
* Testes de integração

---

## 2.2 Fluxo Arquitetural

```
API Controller
      ↓
   MediatR
      ↓
 Command Handler
      ↓
Domain Entities / Repository
      ↓
 Infrastructure
```

---

# 3. Boas Práticas Utilizadas

## 3.1 Padrões de Design

### CQRS

Separação entre:

* **Commands (Write)**
* **Queries (Read)**

### Result Pattern

Padronização no retorno de operações:

* Success
* Failure
* Validation Errors

### Repository Pattern

Abstração do acesso a dados.

### Unit of Work

Controle de transações de forma consistente.

---

## 3.2 Princípios de DDD

### Entidades Ricas

As entidades encapsulam regras de negócio.

### Value Objects

Objetos imutáveis que representam dados compostos:

* `Company`
* `Customer`
* `Product`

### Domain Events

Possibilidade de eventos de domínio através da interface:

```
IDomainEvent
```

### Guard Clauses

Validações defensivas utilizando classes `Guard`.

---

## 3.3 Qualidade de Código

### Injeção de Dependência

Uso de **Dependency Injection (DI)** para desacoplamento.

### Async/Await

Operações assíncronas para melhorar performance.

### Imutabilidade

* Value Objects imutáveis
* Entidades com **setters privados**

### Soft Delete

Exclusão lógica para manter histórico e auditoria.

---

## 3.4 Testabilidade

A arquitetura facilita a criação de testes:

* Separação clara de responsabilidades
* Interfaces permitem **mocking**
* Result Pattern simplifica testes de cenários de sucesso e falha

---

# 4. Necessidade de Criação de Endpoints

Atualmente o `OrderController` possui apenas um endpoint de teste.

Para completar a API, os seguintes **endpoints REST** precisam ser implementados.

---

# 4.1 Endpoints Essenciais

## Criar Pedido

```
POST /api/orders
```

**Body**

* company
* customer
* items

**Resposta**

```
201 Created
```

Retorna o **ID do pedido criado**.

---

## Buscar Pedido por ID

```
GET /api/orders/{id}
```

**Resposta**

```
200 OK
404 Not Found
```

Retorna os **detalhes completos do pedido**.

---

## Listar Pedidos

```
GET /api/orders
```

**Parâmetros**

* status
* data
* empresa
* paginação

**Resposta**

```
200 OK
```

Lista paginada de pedidos.

---

## Atualizar Cabeçalho do Pedido

```
PUT /api/orders/{id}/header
```

**Body**

* company
* customer

**Resposta**

```
200 OK
404 Not Found
```

---

## Adicionar Item ao Pedido

```
POST /api/orders/{id}/items
```

**Body**

* product
* quantity
* price
* discount

**Resposta**

```
201 Created
400 Bad Request (se o pedido não estiver pendente)
```

---

## Atualizar Item do Pedido

```
PUT /api/orders/{id}/items/{itemId}
```

**Resposta**

```
200 OK
404 Not Found
```

---

## Remover Item do Pedido

```
DELETE /api/orders/{id}/items/{itemId}
```

**Resposta**

```
204 No Content
404 Not Found
```

---

## Cancelar Pedido

```
PUT /api/orders/{id}/cancel
```

**Resposta**

```
200 OK
400 Bad Request (se o pedido não puder ser cancelado)
```

---

# 4.2 Considerações para Implementação

Algumas melhorias recomendadas para a API:

### Validação

* `FluentValidation`
* `DataAnnotations`

### Autenticação e Autorização

* JWT Bearer Token

### Paginação

Para endpoints de listagem.

### Versionamento de API

```
/api/v1/orders
```

### Documentação

* Swagger
* OpenAPI

### Logs

* Serilog

### Cache

* Redis para dados frequentemente acessados

### Rate Limiting

Proteção contra abuso de requisições.

---

# 4.3 Próximos Passos

1. Implementar endpoints no `OrderController`
2. Configurar **injeção de dependências**
3. Implementar **repositórios na camada Infrastructure**
4. Adicionar **validações e middlewares**
5. Criar **testes de integração**
6. Configurar **banco de dados (EF Core ou similar)**

---

# Licença

Este projeto segue boas práticas de arquitetura para aplicações **.NET enterprise**.
