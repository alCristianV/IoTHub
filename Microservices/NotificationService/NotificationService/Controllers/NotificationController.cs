using IoTHubAPI.Models;
using IoTHubAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SharpCompress.Common;
using System.Net;

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
        private readonly string _usersServiceUrl;
        private readonly HttpClient _httpClient;

        public NotificationController(INotificationRepository notificationRepository, IConfiguration configuration) {
            _notificationRepository = notificationRepository;
            _usersServiceUrl = configuration.GetValue<string>("Urls:UsersService");
            _httpClient = new HttpClient();
        }

        [ProducesResponseType(typeof(IEnumerable<Notification>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [HttpGet("{userId:length(24)}", Name = "GetUserNotifications")]
        public async Task<IActionResult> GetUserNotifications(string userId) {
            User? user = await GetUserById(userId);
            if (user == null) {
                return NotFound("User not found");
            }
            IEnumerable<Notification> notifications = await _notificationRepository.GetUserNotifications(userId);
            return Ok(notifications);
        }

        [HttpPost("addNotification")]
        public async Task<IActionResult> AddNotification(Notification notification)
        {
            Notification addedNotification = await _notificationRepository.AddNotification(notification);

            return StatusCode(StatusCodes.Status201Created, addedNotification);
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

        private async Task<User?> GetUserById(string userId)
        {
            Request.Headers.TryGetValue("Authorization", out StringValues headerValue);
            Console.WriteLine(headerValue);
            _httpClient.DefaultRequestHeaders.Add("Authorization", headerValue.ToString());

            Console.WriteLine(_usersServiceUrl + userId);
            var response = await _httpClient.GetAsync(_usersServiceUrl + userId);
            Console.WriteLine(response);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(responseContent);
            Console.WriteLine(user);
            return user;
        }
    }
}
