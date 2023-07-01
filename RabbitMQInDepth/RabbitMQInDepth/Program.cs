Console.WriteLine($"Please Choose {Environment.NewLine}1- RabbitMQ Publisher{Environment.NewLine}2- RabbitMQ Consumer");
var rabbitMQMechanism = short.Parse(Console.ReadLine());
await DetermineApplicationMechanism(rabbitMQMechanism);
Console.ReadKey();
async Task DetermineApplicationMechanism(short rabbitMQMechanism)
{
    switch (rabbitMQMechanism)
    {
        case 1:
            await RabbitMQPublisher.HandlePublisherTest();
            break;
        case 2:
            await RabbitMQConsumer.HandleConsumerTest();
            break;
    }
}

