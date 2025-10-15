# LearnWolverine - Docker Setup

This project uses Docker Compose to run RabbitMQ and SQL Server for local development.

## Prerequisites

- Docker Desktop installed and running
- Docker Compose (included with Docker Desktop)

## Services

### RabbitMQ
- **AMQP Port**: 5672
- **Management UI**: http://localhost:15672
- **Username**: guest
- **Password**: guest

### SQL Server
- **Port**: 1433
- **Username**: sa
- **Password**: YourStrong@Passw0rd
- **Edition**: Developer

## Commands

### Start all services
```powershell
docker-compose up -d
```

### View logs
```powershell
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f rabbitmq
docker-compose logs -f sqlserver
```

### Stop all services
```powershell
docker-compose stop
```

### Stop and remove containers
```powershell
docker-compose down
```

### Stop and remove containers + volumes (clean slate)
```powershell
docker-compose down -v
```

### Check service status
```powershell
docker-compose ps
```

## Connection Strings

### RabbitMQ Connection String for Wolverine
```
amqp://guest:guest@localhost:5672
```

### SQL Server Connection String
```
Server=localhost,1433;Database=LearnWolverine;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
```

## Verifying Services

### RabbitMQ
- Open browser: http://localhost:15672
- Login with guest/guest
- You should see the RabbitMQ Management UI

### SQL Server
```powershell
# Using sqlcmd (if installed)
sqlcmd -S localhost,1433 -U sa -P "YourStrong@Passw0rd" -Q "SELECT @@VERSION"
```

## Data Persistence

Data is persisted in Docker volumes:
- `rabbitmq_data` - RabbitMQ data
- `sqlserver_data` - SQL Server databases

These volumes persist even when containers are stopped. Use `docker-compose down -v` to remove them.

## Security Note

⚠️ **Important**: The passwords in this setup are for local development only. Never use these credentials in production!
