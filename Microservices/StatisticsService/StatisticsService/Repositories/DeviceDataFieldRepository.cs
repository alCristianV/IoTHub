using IoTHubAPI.DatabaseSettings;
using IoTHubAPI.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public class DeviceDataFieldRepository : IDeviceDataFieldRepository
    {
        private readonly IMongoCollection<DeviceDataField> _deviceDataFields;

        public async Task<DeviceDataField> GetDataField(string dataFieldId) {
            var field = await _deviceDataFields.FindAsync(d => d.Id == dataFieldId);
            return await field.FirstOrDefaultAsync();
        }
    }
}
