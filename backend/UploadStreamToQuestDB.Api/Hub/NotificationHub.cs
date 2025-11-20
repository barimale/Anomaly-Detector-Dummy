using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class NotificationHub : Hub {
    // Optional: method for clients to call
    public Task SendMessage(string user, string message) {
        return Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
