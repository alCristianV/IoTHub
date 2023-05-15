using System.ComponentModel.DataAnnotations;

namespace IoTHubAPI.Models.Dtos
{
    public class ActionDto
    {
        public string DeviceId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
