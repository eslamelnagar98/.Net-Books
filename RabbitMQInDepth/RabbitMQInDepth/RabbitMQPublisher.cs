﻿using System.Text;
using RabbitMQ.Client;

namespace RabbitMQInDepth;
public static class RabbitMQPublisher
{
    public static void BasicReturn()
    {
        var factory = RabbitMQManager.CreateConnectionFactory();
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        var exchangeName = "BasicReturnExchange";
        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        var queueName = "BasicRetrunQueue";
        channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        var properties = channel.CreateBasicProperties();
        properties.DeliveryMode = 2;
        var routingKey = queueName;
        var messageBody = Encoding.UTF8.GetBytes("Hello, RabbitMQ!");
        channel.BasicReturn += (sender, eventArgs) =>
        {
            var messageBody = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            Console.WriteLine($"Message returned: {messageBody} For Reason : {eventArgs.ReplyText}");
        };
        channel.QueueBind(queueName, exchangeName, routingKey, null);
        channel.BasicPublish(exchangeName, routingKey, mandatory: true, properties, messageBody);
    }

    public static void PublisherConfirmation()
    {
        var factory = RabbitMQManager.CreateConnectionFactory();
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ConfirmSelect();

        channel.BasicAcks += (sender, eventArgs) =>
        {
            Console.WriteLine("Message published successfully: " + eventArgs.DeliveryTag);
        };

        channel.BasicNacks += (sender, eventArgs) =>
        {
            Console.WriteLine("Message not acknowledged: " + eventArgs.DeliveryTag);
        };

        channel.BasicReturn += (sender, eventArgs) =>
        {
            var messageBody = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            Console.WriteLine($"Message returned: {messageBody} for Reason: {eventArgs.ReplyText}");
        };

        var exchangeName = "PublisherConfirmExchange";
        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

        var queueName = "PublisherConfirmQueue";
        channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var messageBody = Encoding.UTF8.GetBytes("Hello, RabbitMQ!");

        channel.QueueBind(queueName, exchangeName, queueName, null);

        var properties = channel.CreateBasicProperties();
        properties.DeliveryMode = 2;
        channel.BasicPublish(exchangeName, queueName, true, properties, messageBody);
    }

    public static void AlternateExchanges()
    {
        var factory = RabbitMQManager.CreateConnectionFactory();
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        var alternateExchangeName = "AlternateExchange";
        var mainExchangeName = "OrignalExchange_Alternate";
        channel.ExchangeDeclare(alternateExchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);
        var arguments = new Dictionary<string, object> { { "alternate-exchange", alternateExchangeName } };
        channel.ExchangeDeclare(mainExchangeName, ExchangeType.Direct, durable: true, autoDelete: false, arguments);
        var queueName = "AlternateExample";
        channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueDeclare($"{queueName}_NotRoutable_1", durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueDeclare($"{queueName}_NotRoutable_2", durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queueName, mainExchangeName, queueName);
        channel.QueueBind($"{queueName}_NotRoutable_1", alternateExchangeName, queueName);
        channel.QueueBind($"{queueName}_NotRoutable_2", alternateExchangeName, queueName);
        var unroutableMessageBytes = Encoding.UTF8.GetBytes("Unroutable Message");
        channel.BasicPublish(mainExchangeName, "unroutable_routing_key", true, null, unroutableMessageBytes);
        var routableMessageBytes = Encoding.UTF8.GetBytes("Routable Message");
        channel.BasicPublish(mainExchangeName, queueName, true, null, routableMessageBytes);
    }

    public static void RabbitMQTransaction()
    {
        var factory = RabbitMQManager.CreateConnectionFactory();
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        try
        {
            var exchangeName = "TransactionExchange";
            var queueName = "TransactionQueue";
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queueName, exchangeName, queueName, arguments: null);
            channel.TxSelect();
            var message = "Hello, RabbitMQ!";
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchangeName, queueName, mandatory: true, basicProperties: null, body);
            throw new Exception("Simulated exception");
            channel.TxCommit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
            channel.TxRollback();
        }
    }

    public static void RabbitMQAtomicTransaction()
    {

        var factory = RabbitMQManager.CreateConnectionFactory();
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        {
            try
            {
                channel.TxSelect();
                var exchangeName = "TransactionExchange";
                var firstQueueName = "TransactionQueue_1";
                var secondQueueName = "TransactionQueue_2";
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
                channel.QueueDeclare(firstQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(firstQueueName, exchangeName, firstQueueName, arguments: null);
                channel.QueueDeclare(secondQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(secondQueueName, exchangeName, secondQueueName, arguments: null);
                var firstMessage = "Message for Queue TransactionQueue_1";
                var firstMessageBody = Encoding.UTF8.GetBytes(firstMessage);
                channel.BasicPublish(exchangeName, firstQueueName, mandatory: true, basicProperties: null, firstMessageBody);
                var secondMessage = "Message for Queue TransactionQueue_2";
                var secondMessageBody = Encoding.UTF8.GetBytes(secondMessage);
                channel.BasicPublish(exchangeName, secondQueueName, mandatory: true, basicProperties: null, secondMessageBody);
                channel.TxCommit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                channel.TxRollback();
            }
        }
    }
}