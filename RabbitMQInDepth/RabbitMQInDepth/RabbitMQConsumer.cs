namespace RabbitMQInDepth;
internal static class RabbitMQConsumer
{
    internal static async Task HandleConsumerTest()
    {
        Console.WriteLine("Which Consumer Mechanism You Want To Use");
        Console.WriteLine($"1- Consumer Tag  {Environment.NewLine}2- Quality Of Service " +
                          $"{Environment.NewLine}3- Acknolodge Multiple Messages At Once. " +
                          $"{Environment.NewLine}4- Transactions With Consumers" +
                          $"{Environment.NewLine}5- DeadLetter Exchange");
        var cosumerMechanism = short.Parse(Console.ReadLine());
        await TestConsumer(cosumerMechanism);
    }
    private static async Task TestConsumer(short publisherMechanism)
    {
        Console.WriteLine("Is Async?");
        var isAsync = short.Parse(Console.ReadLine()) is not 0;
        switch (publisherMechanism)
        {
            case 1:
                ConsumerTag();
                break;
            case 2:
                await QualityOfService(isAsync);
                break;
            case 3:
                AcknolodgeMultipleMessagesAtOnce();
                break;
            case 4:
                TransactionsWithConsumers();
                break;
            case 5:
                DeadLetterExchange();
                break;
        }
        await Task.CompletedTask;
    }

    private static void DeadLetterExchange()
    {
        using var connection = RabbitMQManager.CreateConnectionFactory();
        using var channel = connection.CreateModel();
        var queueName = "PublisherConfirm_DeadLetterTest";
        var deadLetterExchangeName = "DeadLetterExchange_Test";
        var deadLetterQueueName = "DeadLetterQueue_Test";
        var deadLetterRoutingKey = "deadletter-routing-key";
        var mainQueueArguments = new Dictionary<string, object> {
            { "x-dead-letter-exchange", deadLetterExchangeName },
            { "x-dead-letter-routing-key", deadLetterRoutingKey },
            { "x-message-ttl", 30000 }
        };
        channel.ExchangeDeclare(deadLetterExchangeName, ExchangeType.Direct);
        channel.QueueDeclare(deadLetterQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArguments);
        channel.QueueBind(deadLetterQueueName, deadLetterExchangeName, deadLetterRoutingKey);
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, @event) =>
        {
            var message = Encoding.UTF8.GetString(@event.Body.ToArray());
            Console.WriteLine($"Received message: {message}");
            //If You Want To Route Messages To Dead Letter Exchange
            channel.BasicNack(@event.DeliveryTag, false, false); 
            //ProcessConsumedMessage(@event, channel);
        };
        channel.BasicConsume(queueName, false, consumer: consumer);
        Console.WriteLine("Consumer started");
        Console.ReadKey();

    }


    private static void TransactionsWithConsumers()
    {
        using var connection = RabbitMQManager.CreateConnectionFactory();
        using var channel = connection.CreateModel();
        var queueName = "PublisherConfirmQueue";
        channel.TxSelect();
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, @event) =>
        {
            try
            {
                ProcessConsumedMessage(@event, channel);
                var message = Encoding.UTF8.GetString(@event.Body.ToArray());
                if (short.Parse(message) is 20)
                {
                    throw new Exception("Stop The Transaction");
                }
                channel.TxCommit();
            }
            catch (Exception ex)
            {
                channel.TxRollback();
                channel.BasicNack(@event.DeliveryTag, false, true);
            }
        };

        channel.BasicConsume(queueName, autoAck: false, consumer: consumer);
        Console.WriteLine("Consuming messages. Press any key to exit.");
        Console.ReadKey();
    }
    private static void AcknolodgeMultipleMessagesAtOnce()
    {
        using var connection = RabbitMQManager.CreateConnectionFactory();
        using var channel = connection.CreateModel();
        var queueName = "PublisherConfirmQueue";
        channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, @event) =>
        {
            ProcessConsumedMessage(@event, channel);
            //channel.BasicNack(@event.DeliveryTag, false, true);
        };
        channel.BasicConsume(queueName, autoAck: false, consumer);

        Console.WriteLine("Consumers started");
        Console.ReadKey();
    }
    private static async Task QualityOfService(bool isAsync)
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

    private static void ConsumerTag()
    {
        using var connection = RabbitMQManager.CreateConnectionFactory();
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

    private static void ProcessConsumedMessage(BasicDeliverEventArgs @event, IModel channel)
    {
        var message = Encoding.UTF8.GetString(@event.Body.ToArray());
        Console.WriteLine($"Received message: {message}");
        channel.BasicAck(@event.DeliveryTag, multiple: false);
    }


}
