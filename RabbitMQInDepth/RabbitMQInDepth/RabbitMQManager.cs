using RabbitMQ.Client;
namespace RabbitMQInDepth;
public static class RabbitMQManager
{
    public static ConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };
    }
}
