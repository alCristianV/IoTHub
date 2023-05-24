
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTHubAPI.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineDevices = 
            new Dictionary<string, List<string>>();

        public Task DeviceConnected(string deviceConnectionString, string connectionId) {
            lock (OnlineDevices) {
                if (OnlineDevices.ContainsKey(deviceConnectionString)) {
                    OnlineDevices[deviceConnectionString].Add(connectionId);
                }
                else {
                    OnlineDevices.Add(deviceConnectionString, new List<string> { connectionId });
                }
            }

            return Task.CompletedTask;
        }

        public Task DeviceDisconnected(string deviceConnectionString, string connectionId) {
            lock (OnlineDevices) {
                if (!OnlineDevices.ContainsKey(deviceConnectionString)) {
                    return Task.CompletedTask;
                }
                OnlineDevices[deviceConnectionString].Remove(connectionId);
                if(OnlineDevices[deviceConnectionString].Count == 0) {
                    OnlineDevices.Remove(deviceConnectionString);
                }
            }

            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineDevices() {
            string[] onlineDevices;
            lock (OnlineDevices) {
                onlineDevices = OnlineDevices.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }
            return Task.FromResult(onlineDevices);
        }


    }
}
