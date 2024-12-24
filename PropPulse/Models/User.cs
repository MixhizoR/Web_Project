using System.ComponentModel.DataAnnotations;

namespace PropPulse.Models
{
    public class User
    { 

        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(25)]
        public string LastName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)] // Şifre için minimum uzunluk 6 karakter
        public string Password { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string ProfilePhoto { get; set; } = "default_profile_photo.jpg"; // Varsayılan fotoğraf

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
