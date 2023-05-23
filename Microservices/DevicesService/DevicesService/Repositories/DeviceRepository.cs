using IoTHubAPI.DatabaseSettings;
using IoTHubAPI.Models;
using MongoDB.Driver;

namespace IoTHubAPI.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IMongoCollection<Device> _devices;

        public DeviceRepository(IIotHubDatabaseSettings settings) {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.DatabaseName);

            _devices = database.GetCollection<Device>(settings.DevicesCollectionName);
        }

        public async Task<Device> AddDevice(Device device) {
            await _devices.InsertOneAsync(device);
            return device;
        }

        public async Task DeleteDevice(Device device) {
            await _devices.DeleteOneAsync
                (d => d.Id == device.Id);
        }

        public async Task DeleteDevice(string deviceId) {
            await _devices.DeleteOneAsync
                (device => device.Id == deviceId);
        }

        public async Task<Device> GetDevice(string id) {
            var device = await _devices.FindAsync
                (device => device.Id == id);
            return await device.FirstOrDefaultAsync();
        }

        public async Task<Device> GetDeviceAuthorize(string connectionString) {
            var device = await _devices.FindAsync
                (device => device.ConnectionString == connectionString);
            return await device.FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Device>> GetDevices() {
            var devices = await _devices.FindAsync(device => true);
            return await devices.ToListAsync();
        }

        public async Task<IEnumerable<Device>> GetUserDevices(string userId) {
            var devices = await _devices.FindAsync(device => device.UserId == userId || device.Collaborators.Any(c => c.Id == userId));
            return await devices.ToListAsync();
        }

        public async Task UpdateDevice(string deviceId, Device device) {
            await _devices.ReplaceOneAsync
                (d => d.Id == deviceId, device);
        }
    }
}
