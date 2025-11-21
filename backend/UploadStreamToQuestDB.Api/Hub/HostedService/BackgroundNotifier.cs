using Common.RabbitMQ;
using Common.RabbitMQ.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSSql.Infrastructure.Repositories.Abstractions;
using RabbitMQ.Client.Events;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UploadStreamToQuestDB.API.Hub;

public class BackgroundNotifier : BackgroundService {
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<BackgroundNotifier> _logger;
    private readonly IHubContext<LocalesStatusHub, ILocalesStatusHub> _broadcastLocalesStatus;
    private readonly IQueueConsumerService consumer;
    public BackgroundNotifier(IHubContext<NotificationHub> hubContext, ILogger<BackgroundNotifier> logger
        ,IHubContext<LocalesStatusHub, ILocalesStatusHub> broadcastLocalesStatus,
        IServiceScopeFactory factory) {
        _hubContext = hubContext;
        _logger = logger;
        _broadcastLocalesStatus = broadcastLocalesStatus;
        using var scope = factory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IQueueConsumerService>();

        this.consumer = repo;

        AsyncEventHandler<BasicDeliverEventArgs> bo = async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var obj = JsonSerializer.Deserialize<AlgorithmResult>(body);

            if (obj != null && obj is AlgorithmResult) {
                if(obj.SolutionA && obj.SolutionB && obj.SolutionC) {
                    await _broadcastLocalesStatus.Clients.All.OnStartAsync(Guid.NewGuid().ToString());
                } else {
                    await _broadcastLocalesStatus.Clients.All.OnFinishAsync(Guid.NewGuid().ToString());
                }
            }
        };


        this.consumer.StartAsync(bo);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        int counter = 0;

        while (!stoppingToken.IsCancellationRequested) {
            counter++;
            string message = $"Background update #{counter} at {DateTime.Now:T}";

            _logger.LogInformation("Sending: {Message}", message);
            var id = Guid.NewGuid().ToString();

            //try {
            //    // Send to all connected clients
            //    await _broadcastLocalesStatus.Clients.All.OnStartAsync(id);
            //    await Task.Delay(3000);

            //    await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", message, cancellationToken: stoppingToken);
            //} catch (Exception ex) {
            //    _logger.LogError(ex, "Error sending message via SignalR");
            //} finally {
            //    await _broadcastLocalesStatus.Clients.All.OnFinishAsync(id);
            //}

            await Task.Delay(5000, stoppingToken); // Wait 5 seconds
        }
    }
}
