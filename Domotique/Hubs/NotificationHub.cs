using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class NotificationHub : Hub
    {
        public NotificationHub() : base()
        {
        }

        private async void _deviceService_OnMessage(Messages.Queue.Model.DeviceStatusMessage message)
        {
            await Clients.All.SendAsync("DeviceChange", message.ToString());
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}