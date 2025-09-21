using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;
public class User
{
        [Key]
        [StringLength(50)]
        public string Id { get; set; }  // Custom string ID (e.g., U001)

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        // Nullable → because Google login users won’t have password
        public string? PasswordHash { get; set; }

    // Nullable → only set if provider = google
    [StringLength(255)]
    public string? GoogleId { get; set; }

    [Required]
    [StringLength(50)]
    public string Provider { get; set; } = "local";  // local | google

    [Required]
    [StringLength(50)]
    public string Role { get; set; } = "customer";       // user | organiser | admin

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
