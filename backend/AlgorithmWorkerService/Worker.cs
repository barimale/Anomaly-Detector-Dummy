using Christmas.Secret.Gifter.Infrastructure.Repositories.Abstractions;
using Common.RabbitMQ;
using RabbitMQ.Client.Events;
using System.Text.Json;
using UploadStreamToQuestDB.API.Controllers;

namespace AlgorithmWorkerService {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        public Worker(ILogger<Worker> logger,
            IServiceScopeFactory _scopeFactory) {
            _logger = logger;
            this._scopeFactory = _scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Neural Network Hosted Service running.");

            AsyncEventHandler<BasicDeliverEventArgs> bo = async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var obj = JsonSerializer.Deserialize<AlgorithmDetails>(body);

                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IEventRepository>();

                var result = await repo.AddAsync(new Christmas.Secret.Gifter.Infrastructure.Entities.EventEntry() {
                    Id = Guid.NewGuid().ToString()
                }, cancellationToken);
            };

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IQueueConsumerService>();
            await repo.StartAsync(bo, cancellationToken);
        }

        //public override Task StopAsync(CancellationToken cancellationToken) {
        //    return _queueConsumerService.StopAsync(cancellationToken);
        //}
    }
}
