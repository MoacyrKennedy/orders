# Docker Setup - Sales Orders API

## Arquivos Criados

### 1. **Dockerfile**
Arquivo multi-stage para build e deploy da API do sistema de pedidos.

**Características:**
- **Stage 1 (Build)**: Usa `dotnet/sdk:8.0` para compilar a aplicação
- **Stage 2 (Runtime)**: Usa `dotnet/aspnet:8.0` para runtime otimizado
- Health check integrado
- Portas 80 e 443 expostas
- Sem AppHost redundante para melhor compatibility

### 2. **.dockerignore**
Arquivo para otimizar o build context, excluindo arquivos desnecessários.

### 3. **docker/order-compose.yaml**
Arquivo docker-compose para orquestração da aplicação com serviços opcionais.

## Como Usar

### Build da Imagem Docker

#### Opção 1: Build manual
```bash
docker build -t sales-orders-api:latest .
```

#### Opção 2: Build com tag específica
```bash
docker build -t sales-orders-api:1.0 -f Dockerfile .
```

### Executar Container

#### Sem Docker Compose
```bash
docker run \
  -p 5000:80 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  --name sales-orders-api \
  sales-orders-api:latest
```

#### Com Docker Compose (Recomendado)
```bash
cd docker
docker-compose -f order-compose.yaml up -d
```

### Acessar a API
```
http://localhost:5000/order/api
```

## Variáveis de Ambiente

Configure no docker-compose.yaml ou ao executar o container:

| Variável | Descrição | Padrão |
|----------|-----------|--------|
| `ASPNETCORE_ENVIRONMENT` | Ambiente (Development/Production) | Development |
| `ASPNETCORE_URLS` | URLs de escuta | http://+:80 |
| `ConnectionStrings__DefaultConnection` | String de conexão BD | (não configurado) |

## Banco de Dados

### Ativar SQL Server

Descomente as seções no `order-compose.yaml`:

```yaml
depends_on:
  - mssql-db
```

E a seção do `mssql-db`:
```yaml
  mssql-db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ...
```

Então configure a connection string:
```yaml
environment:
  - ConnectionStrings__DefaultConnection=Server=mssql-db;Database=SalesOrdersDb;User Id=sa;Password=YourPassword123!
```

### Ativar PostgreSQL

Descomente as seções no `order-compose.yaml` e use ao invés de SQL Server.

## Comandos Úteis

### Ver logs
```bash
docker-compose -f docker/order-compose.yaml logs -f order-api
```

### Parar container
```bash
docker-compose -f docker/order-compose.yaml down
```

### Remover volume de dados
```bash
docker-compose -f docker/order-compose.yaml down -v
```

### Verificar saúde do container
```bash
docker ps
# ou para mais detalhes
docker inspect sales-orders-api | grep -A 5 Health
```

## Configuração de Produção

Para ambiente de produção:

1. Mude `ASPNETCORE_ENVIRONMENT` para `Production`
2. Use senhas fortes no banco de dados
3. Configure volumes para persistência de dados
4. Implemente reverse proxy (Nginx/Traefik)
5. Configure certificados SSL/TLS
6. Use secrets do Docker ou Kubernetes

Exemplo de docker-compose.prod.yaml:
```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Production
  - ConnectionStrings__DefaultConnection=Server=prod-db-server;Database=SalesOrdersDb;...
```

## Troubleshooting

### Erro: "Address already in use"
```bash
# Mude a porta no docker-compose.yaml
ports:
  - "5001:80"  # Ao invés de 5000
```

### Erro: "Failed to restore"
```bash
# Limpe o cache e tente novamente
docker build --no-cache -t sales-orders-api:latest .
```

### Container sai imediatamente
```bash
# Veja os logs
docker logs sales-orders-api
```

## Performance

- Imagem otimizada: ~200MB (runtime stage apenas)
- Build time: ~2-3 minutos (dependendo da machine)
- Memory footprint: ~100-200MB por container

## Segurança

- Não inclua secrets em hardcoded no Dockerfile
- Use Docker secrets ou variáveis de ambiente
- Mantenha base images atualizadas
- Scaneie vulnerabilidades: `docker scan sales-orders-api:latest`
