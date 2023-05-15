using IoTHubAPI.Enums;
using IoTHubAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> AddMessage(Message message);
        Task<IEnumerable<Message>> GetDeviceMessages(string deviceId);

        Task<IEnumerable<StatisticsEntry>> GetDeviceFieldStatisticsMessages(string deviceId, string fieldId, StatisticsType statisticsType);

        Task DeleteMessage(Message message);
    }
}
