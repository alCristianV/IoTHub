using IoTHubAPI.Models;
using IoTHubAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace IoTHubAPI.Controllers
{
    [Authorize]
    [Route("api/notifications")]
    [ApiController]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), 500)]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;

        public NotificationController(INotificationRepository notificationRepository, IUserRepository userRepository) {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        [ProducesResponseType(typeof(IEnumerable<Notification>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [HttpGet("{userId:length(24)}", Name = "GetUserNotifications")]
        public async Task<IActionResult> GetUserNotifications(string userId) {
            User user = await _userRepository.GetUser(userId);
            if (user == null) {
                return NotFound("User not found");
            }
            IEnumerable<Notification> notifications = await _notificationRepository.GetUserNotifications(userId);
            return Ok(notifications);
        }

        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [HttpDelete("{notificationId:length(24)}")]
        public async Task<IActionResult> Delete(string notificationId) {
            Notification notification = await _notificationRepository.GetNotification(notificationId);

            if (notification == null) {
                return NotFound("Notification not found");
            }

            await _notificationRepository.DeleteNotification(notificationId);

            return Ok();
        }
    }
}
