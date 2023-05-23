using IoTHubAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<Device>> GetDevices();
        Task<Device> GetDevice(string deviceId);
        Task<Device> GetDeviceAuthorize(string connectionString);
        Task<IEnumerable<Device>> GetUserDevices(string userId);
        Task UpdateDevice(string deviceId, Device device);
        Task<Device> AddDevice(Device device);
        Task DeleteDevice(Device device);
        Task DeleteDevice(string deviceId);
    }
}
