using IoTHubAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public interface IActionRepository
    {
        Task<IEnumerable<Models.Action>> GetDeviceActions(string deviceId);
        Task UpdateDeviceAction(string deviceActionId, Models.Action action);
        Task DeleteAction(string actionId);
        Task<Models.Action> AddActionToDevice(Models.Action action);

        Task<Models.Action> GetAction(string actionId);
    }
}
