using Algorithm.Common.ML;
using Algorithm.Common.Model;
using Common.RabbitMQ;
using Common.RabbitMQ.Model;
using MSSql.Infrastructure.Entities;
using MSSql.Infrastructure.Repositories.Abstractions;
using Questdb.Net;
using RabbitMQ.Client.Events;
using System.Text.Json;
using UploadStreamToQuestDB.API.Model;
using UploadStreamToQuestDB.Infrastructure.Utilities;

namespace Algorithm.A.WorkerService {
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

            AsyncEventHandler<BasicDeliverEventArgs> bo = async (model, ea) => {
                try {
                    var body = ea.Body.ToArray();
                    var obj = JsonSerializer.Deserialize<AlgorithmDetailsA>(body);

                    if (obj != null) {
                        // execute algorithm A here
                        // get data from questDB for specific sessionId

                        using var scope = _scopeFactory.CreateScope();
                        var repo = scope.ServiceProvider.GetRequiredService<IEventRepository>();

                        var questDbClient = new QuestDBClient("http://questdb:9000");

                        var request = new PaginationRequest() {
                            PageIndex = 0,
                            PageSize = 1000
                        };
                        var query = BuildQuery(request, obj.SessionId);
                        var queryApi = questDbClient.GetQueryApi();
                        var dataModel = await queryApi.QueryEnumerableAsync<WeatherDataResult>(query);

                        _logger.LogTrace("Data is downloaded.");

                        var ml = scope.ServiceProvider.GetService<ICustomMlContext>();
                        ml.DetectAnomaliesBySpike(dataModel.ToList(), string.Empty);
                        // zrobic zapis do bazy AlgorithmDetailsB
                        var result = await repo.AddAsync(new EventEntry() {
                            Id = Guid.NewGuid().ToString()
                        }, cancellationToken);
                    }
                }catch(Exception ex) {
                    var i = 0;
                }
            };

            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IQueueConsumerService>();
            await repo.StartAsync(bo, cancellationToken);
        }

        //public override Task StopAsync(CancellationToken cancellationToken) {
        //    return _queueConsumerService.StopAsync(cancellationToken);
        //}

        private string BuildQuery(PaginationRequest request, string sessionId) {
            var query = new QueryBuilder()
                .WithSessionId(sessionId)
                .WithDateRange(request.StartDate, request.EndDate)
                .WithPageIndexAndSize(request.PageIndex, request.PageSize)
                .Build();

            return query;
        }
    }
}
