using Domain.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Infrastructure.RabbitMQ;

public class RabbitMQMessageConsumer : IMessageConsumer
{
    private readonly Func<Task<IConnection>> _connectionFactory;
    private readonly IOptions<RabbitMQSettings> _settings;

    public event Func<Guid, Task>? OnMessageReceived;

    public RabbitMQMessageConsumer(Func<Task<IConnection>> connectionFactory, IOptions<RabbitMQSettings> settings)
    {
        _connectionFactory = connectionFactory;
        _settings = settings;
    }

    public async Task ConsumeAsync()
    {
        try
        {
            var connection = await _connectionFactory();

            using var channel = await connection.CreateChannelAsync();
            {

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (sender, eventArgs) =>
                {
                    try
                    {
                        var body = eventArgs.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var id = JsonSerializer.Deserialize<Guid>(message);

                        if (OnMessageReceived != null)
                        {
                            await OnMessageReceived(id);
                            await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                        }
                    }
                    catch (Exception exception)
                    {
                        await channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                    }
                };

                    await channel.BasicConsumeAsync(
                        queue: _settings.Value.Queue,
                        autoAck: true,
                        consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }
}

