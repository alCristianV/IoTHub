using System.ComponentModel.DataAnnotations;

namespace IoTHubAPI.Models.Dtos
{
    public class DeviceForAddDto
    {
        [Required]
        public string Name { get; set; }
    }
}
