using IoTHubAPI.DatabaseSettings;
using IoTHubAPI.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTHubAPI.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IMongoCollection<Notification> _notifications;

        public NotificationRepository(IIotHubDatabaseSettings settings) {
            var client = new MongoClient(settings.ConnectionString);

            var database = client.GetDatabase(settings.DatabaseName);

            _notifications = database.GetCollection<Notification>(settings.NotificationCollectionName);
        }

        public async Task<Notification> AddNotification(Notification notification) {
            await _notifications.InsertOneAsync(notification);
            return notification;
        }

        public async Task DeleteNotification(Notification notification) {
            await _notifications.DeleteOneAsync
                (n => n.Id == notification.Id);
        }

        public async Task DeleteNotification(string notificationId) {
            await _notifications.DeleteOneAsync
                (n => n.Id == notificationId);
        }

        public async Task<Notification> GetNotification(string notificationId) {
            var notification = await _notifications.FindAsync(n => n.Id == notificationId);
            return await notification.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Notification>> GetUserNotifications(string userId) {
            var notifications = await _notifications.FindAsync(n => n.UserId == userId);
            var notificationsList = await notifications.ToListAsync();
            return notificationsList.OrderByDescending(n => n.Date);
        }
    }
}
