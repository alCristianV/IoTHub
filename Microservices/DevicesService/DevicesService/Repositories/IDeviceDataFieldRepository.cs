using IoTHubAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public interface IDeviceDataFieldRepository
    {
        Task<IEnumerable<DeviceDataField>> GetDeviceDataFields(string deviceId);
        Task UpdateDeviceDataField(string deviceDataFieldId, DeviceDataField dataField);
        Task DeleteDataField(string deviceDataFieldId);
        Task<DeviceDataField> AddDataFieldToDevice(DeviceDataField dataField);
        Task<DeviceDataField> GetDataField(string dataFieldId);
    }
}
