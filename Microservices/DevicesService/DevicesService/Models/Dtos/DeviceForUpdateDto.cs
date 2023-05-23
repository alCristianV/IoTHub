
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTHubAPI.Models.Dtos
{
    public class DeviceForUpdateDto
    {
        [Required]
        public string Name { get; set; }
        public List<DeviceDataField> Fields { get; set; }
    }
}
