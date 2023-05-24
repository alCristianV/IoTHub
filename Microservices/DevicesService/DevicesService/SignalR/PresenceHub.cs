using IoTHubAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace IoTHubAPI.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker presenceTracker;

        public PresenceHub(PresenceTracker presenceTracker) {
            this.presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync() {
            await this.presenceTracker.DeviceConnected(Context.User.GetDeviceId(), Context.ConnectionId);
            await Clients.Others.SendAsync("DeviceIsOnline", Context.User.GetDeviceId());

            var currentDevices = await this.presenceTracker.GetOnlineDevices();
            await Clients.All.SendAsync("GetOnlineDevices", currentDevices);
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            await this.presenceTracker.DeviceDisconnected(Context.User.GetDeviceId(), Context.ConnectionId);
            await Clients.Others.SendAsync("DeviceIsOffline", Context.User.GetDeviceId());

            var currentDevices = await this.presenceTracker.GetOnlineDevices();
            await Clients.All.SendAsync("GetOnlineDevices", currentDevices);

            await base.OnDisconnectedAsync(exception);
        }


    }
}
