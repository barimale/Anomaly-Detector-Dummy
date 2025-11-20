using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class BackgroundNotifier : BackgroundService {
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<BackgroundNotifier> _logger;

    public BackgroundNotifier(IHubContext<NotificationHub> hubContext, ILogger<BackgroundNotifier> logger) {
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        int counter = 0;

        while (!stoppingToken.IsCancellationRequested) {
            counter++;
            string message = $"Background update #{counter} at {DateTime.Now:T}";

            _logger.LogInformation("Sending: {Message}", message);

            try {
                // Send to all connected clients
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", message, cancellationToken: stoppingToken);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error sending message via SignalR");
            }

            await Task.Delay(5000, stoppingToken); // Wait 5 seconds
        }
    }
}
