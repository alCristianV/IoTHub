using IoTHubAPI.Models;
using IoTHubAPI.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> AddNotification(Notification notification);
        Task<IEnumerable<Notification>> GetUserNotifications(string userId);
        Task<Notification> GetNotification(string notificationId);
        Task DeleteNotification(Notification notification);
        Task DeleteNotification(string notificationId);
    }
}
