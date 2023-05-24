using IoTHubAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public interface IDeviceDataFieldRepository
    {
        Task<DeviceDataField> GetDataField(string dataFieldId);
    }
}
