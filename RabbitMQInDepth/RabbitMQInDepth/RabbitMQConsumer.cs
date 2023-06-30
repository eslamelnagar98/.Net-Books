using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace RabbitMQInDepth;
public static class RabbitMQConsumer
{
    public static async Task QualityOfService()
    {
        var factory = RabbitMQManager.CreateConnectionFactory();
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        string queueName = "PublisherConfirmQueue";
        channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);
        for (int i = 0; i < 3; i++)
        {
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (_, ea) => await ProcessQualityOfServiceMessage(ea, channel, i);
            channel.BasicConsume(queueName, autoAck: false, consumer);
        }
        Console.WriteLine("Consumers started. Press any key to exit.");
        await Task.CompletedTask;
    }
    public static void ConsumerTag()
    {
        var factory = RabbitMQManager.CreateConnectionFactory();
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        var queueName = "AlternateExample";
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, ea) => ProcessConsumerTagMessage(ea, channel);
        string consumerTag = "consumer1";
        channel.BasicConsume(queueName, autoAck: false, consumerTag, consumer);
        Console.WriteLine("Consumer started.Press any key to exit.");
    }

    private static void ProcessConsumerTagMessage(BasicDeliverEventArgs @event, IModel channel)
    {
        var message = Encoding.UTF8.GetString(@event.Body.ToArray());
        Console.WriteLine($"Received message: {message} from consumer tag: {@event.ConsumerTag}");
        if (@event.ConsumerTag == "consumer1")
        {
            Console.WriteLine("Processing message for consumer1...");
            channel.BasicAck(@event.DeliveryTag, multiple: false);
            return;
        }
        Console.WriteLine("Processing message for consumer2...");
        channel.BasicAck(@event.DeliveryTag, multiple: false);

    }

    private static async Task ProcessQualityOfServiceMessage(BasicDeliverEventArgs @event, IModel channel, int consumerIndex)
    {
        var message = Encoding.UTF8.GetString(@event.Body.ToArray());
        Console.WriteLine($"Consumer Number {consumerIndex + 1} Received message: {message}");
        await Task.Delay(TimeSpan.FromSeconds(2));
        //channel.BasicAck(deliveryTag: @event.DeliveryTag, multiple: false);
    }
}