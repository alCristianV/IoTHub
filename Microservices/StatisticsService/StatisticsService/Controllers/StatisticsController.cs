using System.Net;
using IoTHubAPI.Enums;
using IoTHubAPI.Models;
using IoTHubAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace IoTHubAPI.Controllers
{
    [Authorize]
    [Route("api/statistics")]
    [ApiController]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), 500)]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IDeviceDataFieldRepository _deviceDataFieldRepository;
        private readonly string _devicesServiceUrl;
        private readonly HttpClient _httpClient;
        public StatisticsController(IStatisticsRepository statisticsRepository, IDeviceDataFieldRepository deviceDataFieldRepository, IConfiguration configuration) { 
            _statisticsRepository = statisticsRepository;
            _deviceDataFieldRepository = deviceDataFieldRepository;
            _devicesServiceUrl = configuration.GetValue<string>("Urls:DevicesService");
            _httpClient = new HttpClient();
        }

        [ProducesResponseType(typeof(IEnumerable<StatisticsEntry>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [HttpGet("{deviceId:length(24)}/{fieldId:length(24)}/{statisticsType:int}", Name = "GetDeviceDataFieldStatistics")]
        public async Task<IActionResult> GetDeviceDataFieldStatistics(string deviceId, string fieldId, StatisticsType statisticsType) {
            Device device = await GetDevice(deviceId);
            if (device == null) {
                return NotFound("Device not found");
            }
            DeviceDataField field = await _deviceDataFieldRepository.GetDataField(fieldId);
            if (field == null) {
                return NotFound("Field not found");
            }
            if(field.DeviceId != device.Id) {
                return BadRequest("The field is not a part of the device");
            }
            IEnumerable<StatisticsEntry> values = await _statisticsRepository.GetDeviceFieldStatisticsMessages(deviceId, fieldId, statisticsType);
            return Ok(new { fieldName = field.Name, values });
        }

        private async Task<Device> GetDevice(string deviceId)
        {
            Request.Headers.TryGetValue("Authorization", out StringValues headerValue);
            Console.WriteLine(headerValue);
            _httpClient.DefaultRequestHeaders.Add("Authorization", headerValue.ToString());

            var response = await _httpClient.GetAsync(_devicesServiceUrl + deviceId);

            Console.WriteLine(response);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<Device>(responseContent);
            Console.WriteLine(user);
            return user;
        }
    }
}
