﻿using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class SignupRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "customer";
    }
}
