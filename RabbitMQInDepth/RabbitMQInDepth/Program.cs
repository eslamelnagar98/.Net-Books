using RabbitMQInDepth;
//await TestPublisher();
TestConsumer();
static async Task TestPublisher()
{
    await PublisherConfirms.Create();
    //RabbitMQPublisher.BasicReturn();
    //RabbitMQPublisher.PublisherConfirmation();
    //RabbitMQPublisher.AlternateExchanges();
    //RabbitMQPublisher.RabbitMQTransaction();
}
static async Task TestConsumer()
{
    //RabbitMQConsumer.ConsumerTag();
    await RabbitMQConsumer.QualityOfService();
}

Console.ReadKey();
