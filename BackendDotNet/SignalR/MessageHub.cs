using AutoMapper;
using IoTHubAPI.Extensions;
using IoTHubAPI.Helpers;
using IoTHubAPI.Models;
using IoTHubAPI.Models.Dtos;
using IoTHubAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IoTHubAPI.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IDeviceDataFieldRepository deviceDataFieldRepository;
        private readonly IActionRepository actionsRepository;
        private readonly IMapper mapper;

        public MessageHub(IMessageRepository messageRepository, IMapper mapper, IDeviceDataFieldRepository deviceDataFieldRepository, IActionRepository actionRepository) {
            this.messageRepository = messageRepository;
            this.deviceDataFieldRepository = deviceDataFieldRepository;
            this.actionsRepository = actionRepository;
            this.mapper = mapper;
        }

        public override async Task OnConnectedAsync() {
            var httpContext = Context.GetHttpContext();
            var groupName = httpContext.Request.Query["device"].ToString();
            if(!string.IsNullOrEmpty(groupName)) {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
            else {
                await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.GetDeviceId());
            }
            
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message) {
            //get from message
            var deviceId = Context.User.GetDeviceId();
            var deviceDataFields = await deviceDataFieldRepository.GetDeviceDataFields(deviceId);

            Message messageToAdd = new Message()
            {
                DeviceId = deviceId,
                Values = JsonUtils.JsonToDeviceDataFieldValues(message, deviceDataFields)
            };
            if (messageToAdd.Values != null && messageToAdd.Values.ToArray().Length != 0) {
                await messageRepository.AddMessage(messageToAdd);
                var groupName = deviceId;
                await Clients.OthersInGroup(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(messageToAdd));
            }
        }

        public async Task InvokeAction(Models.Action action) {
            var httpContext = Context.GetHttpContext();
            var deviceId = httpContext.Request.Query["device"].ToString();
            var deviceActions = await actionsRepository.GetDeviceActions(deviceId);
            if (deviceActions.Any(a => a.Id == action.Id)) {
                var groupName = deviceId;
                await Clients.Group(groupName).SendAsync("InvokedAction", action);
                //await Clients.All.SendAsync("InvokedAction", action);
            }
        }
        public async Task SendError(string message) {
            var groupName = Context.User.GetDeviceId();
            await Clients.OthersInGroup(groupName).SendAsync("Error", message);
        }

        public async Task SendFieldError(string message) {
            var groupName = Context.User.GetDeviceId();
            JObject obj = JObject.Parse(message);
            string id = (string)obj["id"];            
            var field = await this.deviceDataFieldRepository.GetDataField(id);
            if(string.IsNullOrEmpty(field.Error)) {
                string error = (string)obj["error"];
                field.Error = error;
                await deviceDataFieldRepository.UpdateDeviceDataField(field.Id, field);
                await Clients.OthersInGroup(groupName).SendAsync("FieldError", message);
            }           
        }

        public async Task ChangedField(bool changed) {
            var httpContext = Context.GetHttpContext();
            var groupName = httpContext.Request.Query["device"].ToString();
            await Clients.All.SendAsync("FieldChanged", changed);
        }

    }
}
