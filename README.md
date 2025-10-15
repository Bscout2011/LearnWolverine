# LearnWolverine

A learning project demonstrating **Wolverine** messaging framework with **RabbitMQ**, **SQL Server**, and **Entity Framework Core** integration.

## Features

- 🚀 **Wolverine Messaging Framework** - Modern .NET messaging with handlers and middleware
- 🐰 **RabbitMQ Transport** - Distributed messaging with auto-provisioning
- 💾 **SQL Server Persistence** - Durable inbox/outbox pattern for reliable messaging
- 🗄️ **Entity Framework Core** - Database-first persistence with migrations
- 📨 **Ping-Pong Flow** - Demonstration of cascading messages through the outbox pattern

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for RabbitMQ and SQL Server)
- [Git](https://git-scm.com/)

## Quick Start

### 1. Clone the Repository

```bash
git clone <your-repo-url>
cd LearnWolverine
```

### 2. Start Infrastructure

Start RabbitMQ and SQL Server using Docker Compose:

```bash
docker-compose up -d
```

This will start:
- **RabbitMQ** on port 5672 (Management UI: http://localhost:15672)
  - Username: `guest`
  - Password: `guest`
- **SQL Server** on port 1433
  - Username: `sa`
  - Password: `YourStrong@Passw0rd`

### 3. Verify Database

The `LearnWolverine` database should be created automatically. If not, create it manually:

```bash
docker exec learnwolverine-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -C -Q "CREATE DATABASE LearnWolverine"
```

### 4. Apply EF Core Migrations

```bash
dotnet ef database update
```

This creates the `Pings` table for storing ping entities.

### 5. Run the Application

```bash
dotnet run
```

The API will start on `http://localhost:5218` (or the port shown in the console).

## Testing the API

### Create a Ping

Send a POST request to create a ping:

```bash
# Using curl
curl -X POST http://localhost:5218/ping \
  -H "Content-Type: application/json" \
  -d '{"text":"Hello Wolverine!"}'

# Using PowerShell
Invoke-RestMethod -Uri http://localhost:5218/ping -Method POST -ContentType "application/json" -Body '{"text":"Hello Wolverine!"}'
```

This triggers the **ping-pong flow**:
1. `CreatePing` → Creates a `Ping` entity in the database
2. `PingCreated` → Published to RabbitMQ via the outbox
3. `Pong` → Response message sent back through the queue
4. The ping is stored in SQL Server and visible in the database

### View All Pings

```bash
# Using curl
curl http://localhost:5218/ping

# Using PowerShell
Invoke-RestMethod -Uri http://localhost:5218/ping
```

### Get Specific Ping

```bash
# Using curl
curl http://localhost:5218/ping/{guid}

# Using PowerShell
Invoke-RestMethod -Uri http://localhost:5218/ping/{guid}
```

## Project Structure

```
LearnWolverine/
├── CreatePingHandler.cs       # Handles CreatePing commands
├── PingCreatedHandler.cs      # Handles PingCreated events
├── PongHandler.cs             # Handles Pong responses
├── PingRepository.cs          # Repository for Ping entities
├── LearnWolverineDbContext.cs # EF Core DbContext
├── Migrations/                # EF Core migrations
├── Program.cs                 # Application startup and configuration
├── appsettings.json           # Configuration (connection strings)
├── docker-compose.yml         # Docker infrastructure
├── DOCKER.md                  # Docker setup documentation
└── PING_PONG_FLOW.md         # Outbox pattern documentation
```

## Architecture

### Message Flow

```
HTTP POST /ping
    ↓
CreatePingHandler
    ↓ (saves to DB + returns PingCreated)
Wolverine Outbox → RabbitMQ Queue 'ping_created'
    ↓
PingCreatedHandler
    ↓ (returns Pong)
Wolverine Outbox → RabbitMQ Queue 'pong'
    ↓
PongHandler (logs completion)
```

### Key Technologies

- **Wolverine**: Message handlers, routing, and transactional middleware
- **RabbitMQ**: Message broker for distributed communication
- **SQL Server**: Durable storage for messages (outbox/inbox) and entities
- **EF Core**: ORM with automatic transaction management via Wolverine

### Wolverine Features Used

- ✅ Auto-discovered handlers
- ✅ Cascading messages (return values become new messages)
- ✅ Durable outbox pattern (transactional message publishing)
- ✅ RabbitMQ integration with auto-provisioning
- ✅ EF Core transaction integration (`UseEntityFrameworkCoreTransactions()`)
- ✅ SQL Server persistence for inbox/outbox

## Configuration

### Connection Strings

Located in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "RabbitMq": "amqp://guest:guest@localhost:5672",
    "SqlServer": "Server=localhost,1433;Database=LearnWolverine;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  }
}
```

### Wolverine Configuration

In `Program.cs`, Wolverine is configured with:
- RabbitMQ transport with auto-provisioning
- SQL Server persistence for outbox/inbox
- EF Core transaction integration
- Message routing to specific queues

## Development

### Add a New Migration

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

### View Database Tables

Connect to SQL Server using the SQL Server extension in VS Code:
- Server: `localhost,1433`
- Database: `LearnWolverine`
- Username: `sa`
- Password: `YourStrong@Passw0rd`

Key tables:
- `dbo.Pings` - Your application data
- `wolverine.wolverine_outgoing_envelopes` - Outbox messages
- `wolverine.wolverine_incoming_envelopes` - Inbox messages

### RabbitMQ Management

Access the RabbitMQ Management UI at http://localhost:15672
- Username: `guest`
- Password: `guest`

View queues:
- `ping`
- `ping_created`
- `pong`

## Stopping the Application

```bash
# Stop the .NET application
Ctrl+C

# Stop Docker containers
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

## Troubleshooting

### Database doesn't exist

Create it manually:
```bash
docker exec learnwolverine-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -C -Q "CREATE DATABASE LearnWolverine"
```

### RabbitMQ connection fails

Ensure Docker container is running:
```bash
docker ps | grep rabbitmq
```

### Pings not being stored

Ensure EF Core migrations are applied:
```bash
dotnet ef database update
```

Check that `UseEntityFrameworkCoreTransactions()` is configured in `Program.cs`.

## Learn More

- [Wolverine Documentation](https://wolverine.netlify.app/)
- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Outbox Pattern](https://microservices.io/patterns/data/transactional-outbox.html)

## License

MIT
