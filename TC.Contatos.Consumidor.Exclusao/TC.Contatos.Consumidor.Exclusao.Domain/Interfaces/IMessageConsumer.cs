using Domain.RegionalAggregate;

namespace Domain.Interfaces;

public interface IMessageConsumer
{
    event Func<Guid, Task>? OnMessageReceived;
    Task ConsumeAsync();
}

