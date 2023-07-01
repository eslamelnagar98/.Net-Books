namespace RabbitMQInDepth;
internal static class RabbitMQConsumer
{
    internal static async Task HandleConsumerTest()
    {
        Console.WriteLine("Which Consumer Mechanism You Want To Use");
        Console.WriteLine($"1- Consumer Tag Test {Environment.NewLine}2- Quality Of Service ");
        var cosumerMechanism = short.Parse(Console.ReadLine());
        await TestConsumer(cosumerMechanism);
    }
    private static async Task TestConsumer(short publisherMechanism)
    {
        Console.WriteLine("Is Async?");
        var isAsync = bool.Parse(Console.ReadLine());
        switch (publisherMechanism)
        {
            case 1:
                ConsumerTag(isAsync);
                break;
            case 2:
                await QualityOfService(isAsync);
                break;
        }
        await Task.CompletedTask;
    }
    public static async Task QualityOfService(bool isAsync)
    {
        using var connection = RabbitMQManager.CreateConnectionFactory(isAsync);
        using var channel = connection.CreateModel();
        var queueName = "PublisherConfirmQueue";
        channel.BasicQos(prefetchSize: 0, prefetchCount: 30, global: false);
        for (int i = 0; i < 3; i++)
        {
            var consumerIndex = i;
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (_, @event) => await ProcessQualityOfServiceMessage(@event, channel, consumerIndex);
            channel.BasicConsume(queueName, autoAck: false, consumer);
        }
        Console.WriteLine("Consumers started");
        Console.ReadKey();
        await Task.CompletedTask;
    }

    private static void ConsumerTag(bool isAsync)
    {
        using var connection = RabbitMQManager.CreateConnectionFactory(isAsync);
        using var channel = connection.CreateModel();
        var queueName = "AlternateExample";
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, ea) => ProcessConsumerTagMessage(ea, channel);
        string consumerTag = "consumer1";
        channel.BasicConsume(queueName, autoAck: false, consumerTag, consumer);
        Console.WriteLine("Consumer started");
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
        //await Task.Delay(TimeSpan.FromMilliseconds(1));
        channel.BasicAck(deliveryTag: @event.DeliveryTag, multiple: false);
        await Task.CompletedTask;
    }
}