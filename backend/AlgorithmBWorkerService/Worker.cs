using Algorithm.B.WorkerService.Service;
using Common.RabbitMQ;
using Common.RabbitMQ.Model;
using MSSql.Infrastructure.Repositories.Abstractions;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Algorithm.B.WorkerService {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IML executorService;

        public Worker(ILogger<Worker> logger,
            IServiceScopeFactory _scopeFactory,
            IML executorService) {
            _logger = logger;
            this._scopeFactory = _scopeFactory;
            this.executorService = executorService;
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
