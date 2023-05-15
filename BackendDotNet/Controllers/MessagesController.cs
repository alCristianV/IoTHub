using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using IoTHubAPI.Models;
using IoTHubAPI.Models.Dtos;
using IoTHubAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoTHubAPI.Controllers
{
    [Authorize]
    [Route("api/messages")]
    [ApiController]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), 500)]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;
        public MessagesController(IMessageRepository messageRepository, IDeviceRepository deviceRepository,
            IMapper mapper) {
            _messageRepository = messageRepository;
            _deviceRepository = deviceRepository;
            _mapper = mapper;
        }

        [ProducesResponseType(typeof(Message), (int)HttpStatusCode.Created)]
        [HttpPost]
        public async Task<IActionResult> CreateMessage(string deviceId, MessageForAddDto message) {
            Device device = await _deviceRepository.GetDevice(deviceId);
            if (device == null) {
                return NotFound("Can`t get device");
            }
            if(deviceId != message.DeviceId) {
                return BadRequest("Wrong device id");
            }
            Message messageToAdd = new Message()
            {
                DeviceId = deviceId,
                Values = message.Values
        };
            await _messageRepository.AddMessage(messageToAdd);
            return Ok(_mapper.Map<MessageDto>(messageToAdd));
        }

    }
}
