namespace Common.RabbitMQ {
    public interface IQueueService
    {
        Task Publish(string message);
    }
}