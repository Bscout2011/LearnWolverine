namespace LearnWolverine;

public static class PongHandler
{
    public static async Task Handle(Pong pong, PingRepository repo)
    {
        Console.WriteLine($"[PongHandler] ✓ Received Pong from outbox!");
        Console.WriteLine($"[PongHandler] PingId: {pong.PingId}");
        Console.WriteLine($"[PongHandler] Response: {pong.Response}");
        
        var ping = await repo.GetPingAsync(pong.PingId);
        if (ping != null)
        {
            Console.WriteLine($"[PongHandler] Original Ping: {ping}");
        }
        else 
        {
            Console.WriteLine($"[PongHandler] Original Ping with Id: {pong.PingId} not found.");
        }
        
        Console.WriteLine("[PongHandler] Ping-Pong flow completed! ✓");
    }
}
