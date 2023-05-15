using AutoMapper;
using IoTHubAPI.Helpers;
using IoTHubAPI.Models;
using IoTHubAPI.Models.Dtos;
using IoTHubAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IoTHubAPI.Controllers
{
    [Authorize]
    [Route("api/devices")]
    [ApiController]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), 500)]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceRepository _devicesRepo;
        private readonly IDeviceDataFieldRepository _devicesDataFieldRepo;
        private readonly IActionRepository _devicesActionsRepo;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _usersRepo;
        private readonly IMapper _mapper;
        public DevicesController(IDeviceRepository devicesRepo, IUserRepository usersRepo, IDeviceDataFieldRepository devicesDataFieldRepo, IActionRepository devicesActionsRepo, IMapper mapper,
            INotificationRepository notificationRepository) {
            _devicesRepo = devicesRepo;
            _usersRepo = usersRepo;
            _devicesDataFieldRepo = devicesDataFieldRepo;
            _devicesActionsRepo = devicesActionsRepo;
            _mapper = mapper;
            _notificationRepository = notificationRepository;
        }

        [ProducesResponseType(typeof(IEnumerable<Device>), (int)HttpStatusCode.OK)]
        [HttpGet(Name = "GetAllDevices")]
        public async Task<IActionResult> GetAllDevices() {
            IEnumerable<Device> devices = await _devicesRepo.GetDevices();
            var user = HttpContext.User;
            return Ok(devices);
        }

        [ProducesResponseType(typeof(Device), (int)HttpStatusCode.OK)]
        [HttpGet("{deviceId:length(24)}", Name = "GetDevice")]
        public async Task<IActionResult> GetDevice(string deviceId) {
            Device device = await _devicesRepo.GetDevice(deviceId);

            if(device == null) {
                return NotFound("Device not found");
            }

            return Ok(device);
        }

        [ProducesResponseType(typeof(IEnumerable<DeviceForListDto>), (int)HttpStatusCode.OK)]
        [HttpGet("GetUserDevices/{userId:length(24)}", Name = "GetUserDevices")]
        public async Task<IActionResult> GetUserDevices(string userId) {
            User user = await _usersRepo.GetUser(userId);
            var con = HttpContext.User;
            if(user == null) {
                return NotFound("User not found");
            }
            IEnumerable<Device> devices = await _devicesRepo.GetUserDevices(userId);

            if (devices == null) {
                return NotFound("Devices not found");
            }
            List<DeviceForListDto> devicesForList = new List<DeviceForListDto>();
            foreach(var device in devices) {
                DeviceForListDto deviceToAdd = _mapper.Map<DeviceForListDto>(device);
                deviceToAdd.User = _mapper.Map<UserForListDto>(await _usersRepo.GetUser(device.UserId));
                devicesForList.Add(deviceToAdd);
            }
            return Ok(devicesForList);
        }

        [ProducesResponseType(typeof(Device), (int)HttpStatusCode.OK)]
        [HttpGet("GetUserDevice/{userId:length(24)}/{deviceId:length(24)}", Name = "GetUserDevice")]
        public async Task<IActionResult> GetUserDevice(string userId, string deviceId) {
            User user = await _usersRepo.GetUser(userId);
            if (user == null) {
                return NotFound("User not found");
            }
            IEnumerable<Device> devices = await _devicesRepo.GetUserDevices(userId);
            Device device = devices.Where(device => device.Id == deviceId).FirstOrDefault();
            
            if (device == null) {
                return NotFound("Device not found");
            }
            return Ok(device);
        }

        [ProducesResponseType(typeof(DeviceForListDto), (int)HttpStatusCode.Created)]
        [HttpPost("{userId:length(24)}")]
        public async Task<IActionResult> AddDeviceToUser(string userId, DeviceForAddDto device) {
            Device deviceToAdd = _mapper.Map<Device>(device);

            User user = await _usersRepo.GetUser(userId);
            if (user == null) {
                return NotFound("Can`t get user");
            }

            deviceToAdd.UserId = userId;
            deviceToAdd.ConnectionString = Generator.EncryptString(userId, DateTime.Now.ToString());
            await _devicesRepo.AddDevice(deviceToAdd);

            DeviceForListDto deviceForList = _mapper.Map<DeviceForListDto>(deviceToAdd);
            deviceForList.User = _mapper.Map<UserForListDto>(user);
            return Ok(deviceForList);
        }

        [ProducesResponseType(typeof(Device), (int)HttpStatusCode.OK)]
        [HttpPost("{deviceId:length(24)}/collaborators")]
        public async Task<IActionResult> AddCollaboratorToDevice(string deviceId, string collaboratorEmail) {
            Device device = await _devicesRepo.GetDevice(deviceId);
            if (device == null) {
                return NotFound("Can`t get device");
            }

            User collaborator = await _usersRepo.GetUserByEmail(collaboratorEmail);
            if (collaborator == null) {
                return NotFound("Can`t get user");
            }
            if (collaborator.Id == device.UserId) {
                return BadRequest("Can`t add Owner of device to collaborators");
            }

            if (device.Collaborators.Any(c => c.Email == collaboratorEmail)) {
                return BadRequest("Collaborator already exists!");
            }
            UserInfoDto collaboratorToAdd = _mapper.Map<UserInfoDto>(collaborator);
            device.Collaborators.Add(collaboratorToAdd);
            await _devicesRepo.UpdateDevice(deviceId, device);

            var (username, userId) = GetUserCredentials();
            await _notificationRepository.AddNotification(new Notification() { UserId = collaborator.Id, Text = $"{username} added you as a collaborator to the {device.Name} device" });
            return Ok(collaboratorToAdd);
        }
        [ProducesResponseType(typeof(Device), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [HttpDelete("{deviceId:length(24)}/collaborators/{collaboratorId:length(24)}")]
        public async Task<IActionResult> RemoveCollaboratorFromDevice(string deviceId, string collaboratorId) {
            Device device = await _devicesRepo.GetDevice(deviceId);
            if (device == null) {
                return NotFound("Can`t get device");
            }

            User collaborator = await _usersRepo.GetUser(collaboratorId);
            if (collaborator == null) {
                return NotFound("Can`t get user");
            }

            if (!device.Collaborators.Any(c => c.Id == collaboratorId)) {
                return BadRequest("Collaborator not found");
            }
            UserInfoDto collaboratorToRemove = _mapper.Map<UserInfoDto>(collaborator);
            device.Collaborators.RemoveAll(c => c.Id == collaboratorId);
            await _devicesRepo.UpdateDevice(deviceId, device);

            await _devicesRepo.UpdateDevice(deviceId, device);
            var (username, userId) = GetUserCredentials();
            await _notificationRepository.AddNotification(new Notification() { UserId = collaboratorId, Text = $"You are no longer a collaborator of {device.Name} device" });
            return Ok(device);
        }

        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpDelete("{deviceId:length(24)}")]
        public async Task<IActionResult> Delete(string deviceId) {
            Device device = await _devicesRepo.GetDevice(deviceId);

            if (device == null) {
                return NotFound("Device not found");
            }

            await _devicesRepo.DeleteDevice(device.Id);

            return Ok();
        }

        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPut("{deviceId:length(24)}")]
        public async Task<IActionResult> Update(string deviceId, [FromBody] DeviceForAddDto updatedDevice) {
            if (updatedDevice == null) {
                return BadRequest(ModelState);
            }

            Device device = await _devicesRepo.GetDevice(deviceId);

            if (device == null) {
                return NotFound();
            }

            var deviceList = _mapper.Map<DeviceForAddDto>(device);
            var (username, userId) = GetUserCredentials();
            if (userId != device.UserId) {
                await _notificationRepository.AddNotification(new Notification() { UserId = device.UserId, Text = $"{username} updated {device.Name} device \n{Newtonsoft.Json.JsonConvert.SerializeObject(deviceList)} \n -----> \n{Newtonsoft.Json.JsonConvert.SerializeObject(updatedDevice)}" });
            }

            device.Name = updatedDevice.Name;
            //table = _mapper.Map<Table>(updatedTableDto);
            //table.Id = id;
            await _devicesRepo.UpdateDevice(deviceId, device);

            return Ok(device);

        }

        [ProducesResponseType(typeof(IEnumerable<DeviceDataField>), (int)HttpStatusCode.OK)]
        [HttpGet("GetDeviceFields/{deviceId:length(24)}", Name = "GetDeviceFields")]
        public async Task<IActionResult> GetDeviceFields(string deviceId) {
            Device device = await _devicesRepo.GetDevice(deviceId);

            if (device == null) {
                return NotFound("Device not found");
            }
            IEnumerable<DeviceDataField> fields = await _devicesDataFieldRepo.GetDeviceDataFields(deviceId);

            if (fields == null) {
                return NotFound("Fields not found");
            }

            return Ok(fields);
        }

        [ProducesResponseType(typeof(IEnumerable<DeviceDataField>), (int)HttpStatusCode.OK)]
        [HttpGet("GetConnDeviceFields", Name = "GetConnDeviceFields")]
        public async Task<IActionResult> GetConnDeviceFields(string connectionString) {
            Device device = await _devicesRepo.GetDeviceAuthorize(connectionString);

            if (device == null) {
                return NotFound("Device not found");
            }
            IEnumerable<DeviceDataField> fields = await _devicesDataFieldRepo.GetDeviceDataFields(device.Id);

            if (fields == null) {
                return NotFound("Fields not found");
            }

            return Ok(fields);
        }

        [ProducesResponseType(typeof(DeviceDataField), (int)HttpStatusCode.OK)]
        [HttpPost("{deviceId:length(24)}/fields")]
        public async Task<IActionResult> AddDataFieldToDevice(string deviceId, DeviceDataFieldDto field) {
            Device device = await _devicesRepo.GetDevice(deviceId);
            if (device == null) {
                return NotFound("Can`t get device");
            }

            DeviceDataField dataField = _mapper.Map<DeviceDataField>(field);
            dataField.DeviceId = deviceId;

            var dataFields =await _devicesDataFieldRepo.GetDeviceDataFields(deviceId);

            if (dataFields.Any(f => f.Name == field.Name)) {
                return NotFound("Name conflict");
            }

            await _devicesDataFieldRepo.AddDataFieldToDevice(dataField);
            var (username, userId) = GetUserCredentials();
            if(userId != device.UserId) {
                var dataFieldList = _mapper.Map<DeviceDataFieldListDto>(dataField);
                await _notificationRepository.AddNotification(new Notification() { UserId = device.UserId, Text = $"{username} added a new DataField to {device.Name} device \n {Newtonsoft.Json.JsonConvert.SerializeObject(dataFieldList)}" });
            }            
            return Ok(dataField);
        }

        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPut("fields/{fieldId:length(24)}")]
        public async Task<IActionResult> UpdateField(string fieldId, [FromBody] DeviceDataFieldDto dataField) {
            if (dataField == null) {
                return BadRequest(ModelState);
            }

            DeviceDataField field = await _devicesDataFieldRepo.GetDataField(fieldId);
            var beforeDataFieldList = _mapper.Map<DeviceDataFieldListDto>(field);
            if (field == null) {
                return NotFound();
            }
            //device.Name = updatedDevice.Name;
            dataField.DeviceId = field.DeviceId;
            field = _mapper.Map<DeviceDataField>(dataField);
            field.Id = fieldId;
            field.Error = null;
            await _devicesDataFieldRepo.UpdateDeviceDataField(fieldId, field);

            var device = await _devicesRepo.GetDevice(field.DeviceId);
            var (username, userId) = GetUserCredentials();
            if (userId != device.UserId) {
                var afterDataFieldList = _mapper.Map<DeviceDataFieldListDto>(field);
                await _notificationRepository.AddNotification(new Notification() { UserId = device.UserId, Text = $"{username} updated a DataField for {device.Name} device. \n{Newtonsoft.Json.JsonConvert.SerializeObject(beforeDataFieldList)} \n -> \n {Newtonsoft.Json.JsonConvert.SerializeObject(afterDataFieldList)}"  });
            }
            return Ok(field);

        }

        [ProducesResponseType(typeof(Device), (int)HttpStatusCode.OK)]
        [HttpDelete("{deviceId:length(24)}/fields/{dataFieldId:length(24)}")]
        public async Task<IActionResult> RemoveDataFieldFromDevice(string deviceId,string dataFieldId) {
            Device device = await _devicesRepo.GetDevice(deviceId);
            if (device == null) {
                return NotFound("Can`t get device");
            }
            var dataFields = await _devicesDataFieldRepo.GetDeviceDataFields(deviceId);
            if (!dataFields.Any(f => f.Id == dataFieldId)) {
                return NotFound("Field not found");
            }

            await _devicesDataFieldRepo.DeleteDataField(dataFieldId);

            return Ok();
        }

        [ProducesResponseType(typeof(IEnumerable<Models.Action>), (int)HttpStatusCode.OK)]
        [HttpGet("GetDeviceActions/{deviceId:length(24)}", Name = "GetDeviceActions")]
        public async Task<IActionResult> GetDeviceActions(string deviceId) {
            Device device = await _devicesRepo.GetDevice(deviceId);

            if (device == null) {
                return NotFound("Device not found");
            }
            IEnumerable<Models.Action> actions = await _devicesActionsRepo.GetDeviceActions(deviceId);

            if (actions == null) {
                return NotFound("Actions not found");
            }

            return Ok(actions);
        }

        [ProducesResponseType(typeof(Models.Action), (int)HttpStatusCode.OK)]
        [HttpPost("{deviceId:length(24)}/actions")]
        public async Task<IActionResult> AddActionToDevice(string deviceId, ActionDto action) {
            Device device = await _devicesRepo.GetDevice(deviceId);
            if (device == null) {
                return NotFound("Can`t get device");
            }

            Models.Action actionToAdd = _mapper.Map<Models.Action>(action);
            actionToAdd.DeviceId = deviceId;

            var actions = await _devicesActionsRepo.GetDeviceActions(deviceId);

            if (actions.Any(a => a.Name == action.Name)) {
                return NotFound("Name conflict");
            }

            await _devicesActionsRepo.AddActionToDevice(actionToAdd);
            var (username, userId) = GetUserCredentials();
            if (userId != device.UserId) {
                var actionList = _mapper.Map<ActionListDto>(action);
                await _notificationRepository.AddNotification(new Notification() { UserId = device.UserId, Text = $"{username} added a new Action to {device.Name} device \n {Newtonsoft.Json.JsonConvert.SerializeObject(actionList)}" });
            }
            return Ok(actionToAdd);
        }

        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPut("actions/{actionId:length(24)}")]
        public async Task<IActionResult> UpdateAction(string actionId, [FromBody] ActionDto action) {
            if (action == null) {
                return BadRequest(ModelState);
            }

            Models.Action actionToUpdate = await _devicesActionsRepo.GetAction(actionId);
            
            if (actionToUpdate == null) {
                return NotFound();
            }
            var beforeAction = _mapper.Map<ActionListDto>(actionToUpdate);
            //device.Name = updatedDevice.Name;
            action.DeviceId = actionToUpdate.DeviceId;
            actionToUpdate = _mapper.Map<Models.Action>(action);
            actionToUpdate.Id = actionId;
            await _devicesActionsRepo.UpdateDeviceAction(actionId, actionToUpdate);

            var device = await _devicesRepo.GetDevice(action.DeviceId);
            var (username, userId) = GetUserCredentials();
            if (userId != device.UserId) {
                var afterAction = _mapper.Map<ActionListDto>(actionToUpdate);
                await _notificationRepository.AddNotification(new Notification() { UserId = device.UserId, 
                    Text = $"{username} updated a Action for {device.Name} device. \n {Newtonsoft.Json.JsonConvert.SerializeObject(beforeAction)} \n -> \n{Newtonsoft.Json.JsonConvert.SerializeObject(afterAction)}" });
            }
            return Ok(actionToUpdate);

        }

        [ProducesResponseType(typeof(Device), (int)HttpStatusCode.OK)]
        [HttpDelete("{deviceId:length(24)}/actions/{actionId:length(24)}")]
        public async Task<IActionResult> RemoveActionFromDevice(string deviceId,string actionId) {
            Device device = await _devicesRepo.GetDevice(deviceId);
            if (device == null) {
                return NotFound("Can`t get device");
            }
            var actions = await _devicesActionsRepo.GetDeviceActions(deviceId);
            if (!actions.Any(a => a.Id == actionId)) {
                return NotFound("Field not found");
            }

            await _devicesActionsRepo.DeleteAction(actionId);

            return Ok();
        }

        private (string,string) GetUserCredentials() {
            var username = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Name)?.Value.ToString();
            var userId = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier)?.Value.ToString();
            return (username, userId);
        }


    }
}