using IoTHubAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public interface IActionRepository
    {
        Task<IEnumerable<Action>> GetDeviceActions(string deviceId);
        Task UpdateDeviceAction(string deviceActionId, Action action);
        Task DeleteAction(string actionId);
        Task<Action> AddActionToDevice(Action action);

        Task<Action> GetAction(string actionId);
    }
}
