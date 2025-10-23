using LearnWolverine;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.RabbitMQ;
using Wolverine.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add RabbitMQ as the Wolverine transport
var rabbitMqConnectionString =
    builder.Configuration.GetConnectionString("RabbitMq")
    ?? throw new NullReferenceException("RabbitMq connection string is not configured.");
builder.Host.UseWolverine(opts =>
{
    var rabbit = opts.UseRabbitMq(rabbitMqConnectionString);
    rabbit.AutoProvision();

    // Publish CreatePing messages to the 'ping' queue
    opts.PublishMessage<CreatePing>().ToRabbitQueue("ping");

    // Listen for CreatePing messages from the 'ping' queue
    opts.ListenToRabbitQueue("ping");

    // Publish PingCreated messages to the 'ping_created' queue
    opts.PublishMessage<PingCreated>().ToRabbitQueue("ping_created");

    // Listen for PingCreated messages from the 'ping_created' queue
    opts.ListenToRabbitQueue("ping_created");

    // Publish Pong messages to the 'pong' queue (via outbox)
    opts.PublishMessage<Pong>().ToRabbitQueue("pong");

    // Listen for Pong messages from the 'pong' queue
    opts.ListenToRabbitQueue("pong");

    // Enable SQL Server-backed durable inbox and outbox
    opts.PersistMessagesWithSqlServer(
        builder.Configuration.GetConnectionString("SqlServer")!,
        "wolverine"
    );

    // Enable EF Core integration to automatically call SaveChanges on DbContext
    opts.UseEntityFrameworkCoreTransactions();
    opts.Policies.AutoApplyTransactions();
});

// Add Entity Framework Core DbContext
builder.Services.AddDbContext<LearnWolverineDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer"),
        sqlOptions =>
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            )
    )
);

builder.Services.AddScoped<PingRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();

app.UseHttpsRedirection();

app.MapGet("/", () => "LearnWolverine is running...");

app.MapPost(
    "ping",
    (CreatePing ping, IMessageBus bus) =>
    {
        bus.InvokeAsync(ping);
    }
);

app.MapGet("ping", async (PingRepository repo) => await repo.GetPingsAsync());

app.MapGet("ping/{id:guid}", async (Guid id, PingRepository repo) => await repo.GetPingAsync(id));

app.Run();
