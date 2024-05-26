using System.ComponentModel.DataAnnotations;

namespace MultiFamilyPortal.FirstRun.Models
{
    internal class FirstRunSetup
    {
        public string SiteTitle { get; set; }

        public string SenderEmailName { get; set; }

        [EmailAddress]
        public string SenderEmail { get; set; }

        public string Twitter { get; set; }

        public string Facebook { get; set; }

        public string LinkedIn { get; set; }

        public string Instagram { get; set; }

        public string LegalName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

        [EmailAddress]
        public string PublicEmail { get; set; }

        public string Phone { get; set; }

        [EmailAddress]
        public string AdminUser { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AdminPhone { get; set; }

        public bool UsePassword { get; set; }

        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
