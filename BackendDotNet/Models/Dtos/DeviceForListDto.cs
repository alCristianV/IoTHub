using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IoTHubAPI.Models.Dtos
{
    public class DeviceForListDto
    {
        [Required]
        public string Id { get; set; }

        public UserForListDto User { get; set; }

        public string Name { get; set; }

        public bool Online { get; set; }
    }
}
