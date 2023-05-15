using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using IoTHubAPI.Enums;
using IoTHubAPI.Models;
using IoTHubAPI.Models.Dtos;
using IoTHubAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        private readonly IMessageRepository _messageRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDeviceDataFieldRepository _deviceDataFieldRepository;
        public StatisticsController(IMessageRepository messageRepository, IDeviceRepository deviceRepository,
            IDeviceDataFieldRepository deviceDataFieldRepository) {
            _messageRepository = messageRepository;
            _deviceRepository = deviceRepository;
            _deviceDataFieldRepository = deviceDataFieldRepository;
        }

        [ProducesResponseType(typeof(IEnumerable<StatisticsEntry>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [HttpGet("{deviceId:length(24)}/{fieldId:length(24)}/{statisticsType:int}", Name = "GetDeviceDataFieldStatistics")]
        public async Task<IActionResult> GetDeviceDataFieldStatistics(string deviceId, string fieldId, StatisticsType statisticsType) {
            Device device = await _deviceRepository.GetDevice(deviceId);
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
            IEnumerable<StatisticsEntry> values = await _messageRepository.GetDeviceFieldStatisticsMessages(deviceId, fieldId, statisticsType);
            return Ok(new { fieldName = field.Name, values });
        }
    }
}
