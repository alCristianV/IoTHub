using IoTHubAPI.Enums;
using IoTHubAPI.Models;

namespace IoTHubAPI.Repositories
{
    public interface IStatisticsRepository
    {
        Task<IEnumerable<StatisticsEntry>> GetDeviceFieldStatisticsMessages(string deviceId, string fieldId, StatisticsType statisticsType);
    }
}
