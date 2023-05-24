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
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly IMongoCollection<Message> _messages;

        public StatisticsRepository(IIotHubDatabaseSettings settings) {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.DatabaseName);

            _messages = database.GetCollection<Message>(settings.MessagesCollectionName);
        }

        public async Task<IEnumerable<StatisticsEntry>> GetDeviceFieldStatisticsMessages(string deviceId, string fieldId, StatisticsType statisticsType) {
            var messages = await _messages.FindAsync
                (msg => msg.DeviceId == deviceId && msg.Values.Any(v => v.DataField.Id == fieldId && v.DataField.Type == DataType.Numeric.ToString()));
            if(messages == null) {
                return null;
            }
            var messagesList= await messages.ToListAsync();
            var values = new List<StatisticsEntry>();

            switch (statisticsType) {
                case StatisticsType.Daily:
                    for (int i = 0; i < 24; i++) {
                        var startDate = DateTime.Now.AddDays(-1).AddHours(i);
                        var endDate = DateTime.Now.AddDays(-1).AddHours(i + 1);
                        var hourMsg = messagesList.Where(message => message.Date < endDate && message.Date > startDate);
                        if (hourMsg.Any()) {
                            double hourAverage = hourMsg.Select(m => m.Values.Average(v => double.Parse(v.Value))).ToList().Average();
                            values.Add(new StatisticsEntry() { Value = hourAverage, Date = endDate });
                        }
                    }
                    break;
                case StatisticsType.Monthly:
                    for (int i = 0; i < 12; i++) {
                        var startDate = DateTime.Now.AddYears(-1).AddMonths(i);
                        var endDate = DateTime.Now.AddYears(-1).AddMonths(i + 1);
                        var hourMsg = messagesList.Where(message => message.Date < endDate && message.Date > startDate);
                        if (hourMsg.Any()) {
                            double hourAverage = hourMsg.Select(m => m.Values.Average(v => double.Parse(v.Value))).ToList().Average();
                            values.Add(new StatisticsEntry() { Value = hourAverage, Date = endDate });
                        }
                    }
                    break;
                case StatisticsType.Weekly:
                    for (int i = 0; i < 7; i++) {
                        var startDate = DateTime.Now.AddDays(-7 + i);
                        var endDate = DateTime.Now.AddDays(-7 + i + 1);
                        var hourMsg = messagesList.Where(message => message.Date < endDate && message.Date > startDate);
                        if (hourMsg.Any()) {
                            double hourAverage = hourMsg.Select(m => m.Values.Average(v => double.Parse(v.Value))).ToList().Average();
                            values.Add(new StatisticsEntry() { Value = hourAverage, Date = endDate });
                        }
                    }
                    break;

            }

            
            return values.OrderBy(v => v.Date);
        }

        public async Task<IEnumerable<Message>> GetDeviceMessages(string deviceId) {
            var messages = await _messages.FindAsync
                (msg => msg.DeviceId == deviceId);
            return await messages.ToListAsync();
        }
    }
}
