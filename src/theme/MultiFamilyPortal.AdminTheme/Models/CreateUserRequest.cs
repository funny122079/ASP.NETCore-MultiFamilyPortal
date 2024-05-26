using System.ComponentModel.DataAnnotations;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class CreateUserRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required]
        public List<string> Roles { get; set; }

        public bool UseLocalAccount { get; set; }
    }
}
