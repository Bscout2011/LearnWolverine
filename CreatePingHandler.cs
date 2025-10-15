namespace LearnWolverine;

public class CreatePingHandler(PingRepository repo)
{
    public PingCreated Handle(CreatePing command)
    {
        var ping = new Ping(Guid.NewGuid(), command.Text, DateTime.UtcNow);
        repo.Store(ping);
        Console.WriteLine($"[CreatePingHandler] Created ping: {ping}");
        return new PingCreated(ping.Id, ping.Message);
    }
}

public record CreatePing(string Text);

public record PingCreated(Guid Id, string Message);

