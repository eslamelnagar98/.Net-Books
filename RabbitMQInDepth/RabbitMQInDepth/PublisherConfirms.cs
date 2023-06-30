using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQInDepth;
public static class PublisherConfirms
{
    private const int MESSAGE_COUNT = 50;
    private static readonly ConcurrentDictionary<ulong, string> _outstandingConfirms = new();
    internal static async Task Create()
    {
        //PublishMessagesIndividually();
        //PublishMessagesInBatch();
        await HandlePublishConfirmsAsynchronously();
    }
    private static IConnection CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };
        return factory.CreateConnection();
    }

    private static void PublishMessagesIndividually()
    {
        using var connection = CreateConnection();
        using var channel = connection.CreateModel();

        // declare a server-named queue
        var queueName = channel.QueueDeclare().QueueName;
        channel.ConfirmSelect();

        var startTime = Stopwatch.GetTimestamp();

        for (int i = 0; i < MESSAGE_COUNT; i++)
        {
            var body = Encoding.UTF8.GetBytes(i.ToString());
            channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body);
            channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
        }

        var endTime = Stopwatch.GetTimestamp();

        Console.WriteLine($"Published {MESSAGE_COUNT:N0} messages individually in {Stopwatch.GetElapsedTime(startTime, endTime).TotalMilliseconds:N0} ms");
    }

    private static void PublishMessagesInBatch()
    {
        using var connection = CreateConnection();
        using var channel = connection.CreateModel();

        // declare a server-named queue
        var queueName = channel.QueueDeclare().QueueName;
        channel.ConfirmSelect();

        var batchSize = 100;
        var outstandingMessageCount = 0;

        var startTime = Stopwatch.GetTimestamp();

        for (int i = 0; i < MESSAGE_COUNT; i++)
        {
            var body = Encoding.UTF8.GetBytes(i.ToString());
            channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body);
            outstandingMessageCount++;

            if (outstandingMessageCount == batchSize)
            {
                channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
                outstandingMessageCount = 0;
            }
        }

        if (outstandingMessageCount > 0)
            channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));

        var endTime = Stopwatch.GetTimestamp();

        Console.WriteLine($"Published {MESSAGE_COUNT:N0} messages in batch in {Stopwatch.GetElapsedTime(startTime, endTime).TotalMilliseconds:N0} ms");
    }

    private static async Task HandlePublishConfirmsAsynchronously()
    {
        using var connection = CreateConnection();
        using var channel = connection.CreateModel();

        var exchangeName = "PublisherConfirmExchange";
        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

        var queueName = "PublisherConfirmQueue";
        channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var messageBody = Encoding.UTF8.GetBytes("Hello, RabbitMQ!");

        channel.QueueBind(queueName, exchangeName, queueName, null);

        var properties = channel.CreateBasicProperties();
        properties.DeliveryMode = 2;
        channel.ConfirmSelect();
        channel.BasicAcks += (sender, ea) => CleanOutstandingConfirms(ea.DeliveryTag, ea.Multiple);
        channel.BasicNacks += (sender, ea) =>
        {
            _outstandingConfirms.TryGetValue(ea.DeliveryTag, out string? body);
            Console.WriteLine($"Message with body {body} has been nack-ed. Sequence number: {ea.DeliveryTag}, multiple: {ea.Multiple}");
            CleanOutstandingConfirms(ea.DeliveryTag, ea.Multiple);
        };

        var startTime = Stopwatch.GetTimestamp();

        for (int i = 0; i < MESSAGE_COUNT; i++)
        {
            var body = (i + 1).ToString();
            _outstandingConfirms.TryAdd(channel.NextPublishSeqNo, i.ToString());
            channel.BasicPublish(exchangeName, queueName, true, properties, body: Encoding.UTF8.GetBytes(body));
        }

        if (!await WaitUntil(60, () => _outstandingConfirms.IsEmpty))
        {
            throw new Exception("All messages could not be confirmed in 60 seconds");
        }

        var endTime = Stopwatch.GetTimestamp();
        Console.WriteLine($"Published {MESSAGE_COUNT:N0} messages and handled confirm asynchronously {Stopwatch.GetElapsedTime(startTime, endTime).TotalMilliseconds:N0} ms");
    }

    private static void CleanOutstandingConfirms(ulong sequenceNumber, bool multiple)
    {
        if (multiple)
        {
            var confirmed = _outstandingConfirms.Where(k => k.Key <= sequenceNumber);
            foreach (var entry in confirmed)
            {
                _outstandingConfirms.TryRemove(entry.Key, out _);
            }
        }
        else
            _outstandingConfirms.TryRemove(sequenceNumber, out _);
    }

    private static async ValueTask<bool> WaitUntil(int numberOfSeconds, Func<bool> condition)
    {
        int waited = 0;
        while (!condition() && waited < numberOfSeconds * 1000)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            waited += 100;
        }

        return condition();
    }
}
