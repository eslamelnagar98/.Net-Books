namespace RabbitMQInDepth;
public static class RabbitMQManager
{
    public static IConnection CreateConnectionFactory(bool isAsync)
    {
        return CreateFactoryObject(isAsync).CreateConnection();
    }
    public static IConnection CreateConnectionFactory()
    {
        return CreateFactoryObject().CreateConnection();
    }

    private static ConnectionFactory CreateFactoryObject(bool isAsync = false)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest",
        };

        if (isAsync)
        {
            factory.DispatchConsumersAsync = true;
        }
        return factory;
    }
}
