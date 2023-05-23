using IoTHubAPI.DatabaseSettings;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public class ActionRepository : IActionRepository
    {
        private readonly IMongoCollection<Models.Action> _actions;

        public ActionRepository(IIotHubDatabaseSettings settings) {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.DatabaseName);

            _actions = database.GetCollection<Models.Action>(settings.ActionsCollectionName);
        }

        public async Task<Models.Action> AddActionToDevice(Models.Action action) {
            await _actions.InsertOneAsync(action);
            return action;
        }

        public async Task<Models.Action> GetAction(string actionId) {
            var action = await _actions.FindAsync(d => d.Id == actionId);
            return await action.FirstOrDefaultAsync();
        }

        public async Task DeleteAction(string actionId) {
            await _actions.DeleteOneAsync
                (action => action.Id == actionId);
        }

        public async Task<IEnumerable<Models.Action>> GetDeviceActions(string deviceId) {
            var actions = await _actions.FindAsync(action => action.DeviceId == deviceId);
            return await actions.ToListAsync();
        }

        public async Task UpdateDeviceAction(string deviceActionId, Models.Action action) {
            await _actions.ReplaceOneAsync
                (a => a.Id == deviceActionId, action);
        }
    }
}
