using Algorithm.C.WorkerService.Service;
using Common.RabbitMQ;
using Common.RabbitMQ.Model;
using MSSql.Infrastructure.Entities;
using MSSql.Infrastructure.Repositories.Abstractions;
using Questdb.Net;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Algorithm.C.WorkerService {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        //private readonly IML executorService;

        public Worker(ILogger<Worker> logger,
            IServiceScopeFactory _scopeFactory) {
            _logger = logger;
            this._scopeFactory = _scopeFactory;
            //this.executorService = executorService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Neural Network Hosted Service running.");

            AsyncEventHandler<BasicDeliverEventArgs> bo = async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var obj = JsonSerializer.Deserialize<AlgorithmDetailsC>(body);


                if (obj != null && obj is AlgorithmDetailsC)    {
                    // execute algorithm C here
                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                    Random random = new Random();

                    var next = random.Next(0, 10);
                    var algorithmResult = next > 5;

                    var result = await repo.AddAsync(new EventEntry() {
                        Id = Guid.NewGuid().ToString()
                    }, cancellationToken);
                }
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
