using API.SlowTrainMachineLearning.Services;
using Christmas.Secret.Gifter.Infrastructure.Repositories.Abstractions;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace AlgorithmWorkerService {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly IEventRepository eventRepository;
        private readonly IQueueConsumerService _queueConsumerService;

        public Worker(ILogger<Worker> logger,
            IEventRepository eventRepository,
            IQueueConsumerService queueConsumerService) {
            _logger = logger;
            this.eventRepository = eventRepository;
            this._queueConsumerService = queueConsumerService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Neural Network Hosted Service running.");

            AsyncEventHandler<BasicDeliverEventArgs> bo = async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var obj = JsonSerializer.Deserialize<RegisterModelCommand>(body);
                var result = await eventRepository.AddAsync(new Christmas.Secret.Gifter.Infrastructure.Entities.EventEntry() {
                    Id = Guid.NewGuid().ToString()
                }, cancellationToken);
            };

            await _queueConsumerService.StartAsync(bo, cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            return _queueConsumerService.StopAsync(cancellationToken);
        }
    }
}
