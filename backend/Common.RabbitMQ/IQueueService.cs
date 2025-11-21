namespace Common.RabbitMQ {
    public interface IQueueService
    {
        Task Publish(string message);
        Task Publish<T>(T message);
    }
}
