using System.ComponentModel.DataAnnotations;

namespace IoTHubAPI.Models.Dtos
{
    public class DeviceDataFieldDto
    {
        public string DeviceId { get; set; }

        [Required]
        public string Name { get; set; }
        //public DataType Type { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public bool Statistics { get; set; }
    }
}
