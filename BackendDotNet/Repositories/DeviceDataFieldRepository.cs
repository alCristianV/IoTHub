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

        public DeviceDataFieldRepository(IIotHubDatabaseSettings settings) {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.DatabaseName);

            _deviceDataFields = database.GetCollection<DeviceDataField>(settings.DeviceDataFieldsCollectionName);
        }

        public async Task<DeviceDataField> AddDataFieldToDevice(DeviceDataField dataField) {
            await _deviceDataFields.InsertOneAsync(dataField);
            return dataField;
        }

        public async Task DeleteDataField(string deviceDataFieldId) {
            await _deviceDataFields.DeleteOneAsync
                (dataField => dataField.Id == deviceDataFieldId);
        }

        public async Task<DeviceDataField> GetDataField(string dataFieldId) {
            var field = await _deviceDataFields.FindAsync(d => d.Id == dataFieldId);
            return await field.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DeviceDataField>> GetDeviceDataFields(string deviceId) {
            var dataFields = await _deviceDataFields.FindAsync(dataField => dataField.DeviceId == deviceId);
            return await dataFields.ToListAsync();
        }

        public async Task UpdateDeviceDataField(string deviceDataFieldId, DeviceDataField dataField) {
            await _deviceDataFields.ReplaceOneAsync
                (d => d.Id == deviceDataFieldId, dataField);
        }
    }
}
