﻿using System.ComponentModel.DataAnnotations;

namespace IoTHubAPI.Models.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Not a valid email adress")]
        public string Email { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 4, ErrorMessage = "You must specify password between 4 and 25 characters")]
        public string Password { get; set; }
    }
}
