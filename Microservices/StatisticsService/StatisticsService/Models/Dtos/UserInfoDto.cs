using System.ComponentModel.DataAnnotations;

namespace IoTHubAPI.Models.Dtos
{
    public class UserInfoDto
    {
        [Required]
        public string Id { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
    }
}
