using AutoMapper;
using IoTHubAPI.DatabaseSettings;
using IoTHubAPI.Enums;
using IoTHubAPI.Models;
using IoTHubAPI.Models.Dtos;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMongoCollection<Message> _messages;

        public MessageRepository(IIotHubDatabaseSettings settings) {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.DatabaseName);

            _messages = database.GetCollection<Message>(settings.MessagesCollectionName);
        }
        public async Task<Message> AddMessage(Message message) {
            await _messages.InsertOneAsync(message);
            return message;
        }

        public async Task DeleteMessage(Message message) {
            await _messages.DeleteOneAsync
                (d => d.Id == message.Id);
        }


        public async Task<IEnumerable<Message>> GetDeviceMessages(string deviceId) {
            var messages = await _messages.FindAsync
                (msg => msg.DeviceId == deviceId);
            return await messages.ToListAsync();
        }
    }
}
