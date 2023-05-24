using System;
using System.Collections.Generic;

namespace IoTHubAPI.Models.Dtos
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        //public string MessageText { get; set; }
        public List<DeviceDataFieldValue> Values { get; set; }
        public DateTime Date { get; set; }
    }
}
