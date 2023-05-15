using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace IoTHubAPI.Models.Dtos
{
    public class MessageForAddDto
    {
        public string DeviceId { get; set; }
        public List<DeviceDataFieldValue> Values { get; set; }
    }
}
