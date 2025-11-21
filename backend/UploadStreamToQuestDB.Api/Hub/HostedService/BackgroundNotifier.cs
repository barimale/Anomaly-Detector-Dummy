using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using UploadStreamToQuestDB.API.Hub;

public class BackgroundNotifier : BackgroundService {
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<BackgroundNotifier> _logger;
    private readonly IHubContext<LocalesStatusHub, ILocalesStatusHub> _broadcastLocalesStatus;

    public BackgroundNotifier(IHubContext<NotificationHub> hubContext, ILogger<BackgroundNotifier> logger
        ,IHubContext<LocalesStatusHub, ILocalesStatusHub> broadcastLocalesStatus) {
        _hubContext = hubContext;
        _logger = logger;
        _broadcastLocalesStatus = broadcastLocalesStatus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        int counter = 0;

        while (!stoppingToken.IsCancellationRequested) {
            counter++;
            string message = $"Background update #{counter} at {DateTime.Now:T}";

            _logger.LogInformation("Sending: {Message}", message);
            var id = Guid.NewGuid().ToString();

            try {
                // Send to all connected clients
                await _broadcastLocalesStatus.Clients.All.OnStartAsync(id);
                await Task.Delay(3000);

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", message, cancellationToken: stoppingToken);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error sending message via SignalR");
            } finally {
                await _broadcastLocalesStatus.Clients.All.OnFinishAsync(id);
            }

            await Task.Delay(5000, stoppingToken); // Wait 5 seconds
        }
    }
}
