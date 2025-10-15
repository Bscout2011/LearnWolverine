namespace LearnWolverine;

public static class PingCreatedHandler
{
    // This handler processes PingCreated and returns a Pong message
    // The Pong will be stored in the outbox and sent to RabbitMQ
    public static async Task<Pong> Handle(PingCreated pingCreated, PingRepository repo)
    {
        var ping = await repo.GetPingAsync(pingCreated.Id);
        if (ping != null)
        {
            Console.WriteLine($"[PingCreatedHandler] Received PingCreated: {ping}");
            Console.WriteLine($"[PingCreatedHandler] Publishing Pong message to outbox...");
        }
        else
        {
            Console.WriteLine($"[PingCreatedHandler] Ping with Id: {pingCreated.Id} not found.");
        }
        
        return new Pong(pingCreated.Id, $"Pong response to: {pingCreated.Message}");
    }
}

public record Pong(Guid PingId, string Response);

