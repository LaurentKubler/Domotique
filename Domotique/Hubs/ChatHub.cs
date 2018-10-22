using Domotique.Service;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        IDeviceStatusReadingService _deviceService;


        public ChatHub(IDeviceStatusReadingService deviceService) : base()
        {
            _deviceService = deviceService;
            _deviceService.OnMessage += _deviceService_OnMessage;
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