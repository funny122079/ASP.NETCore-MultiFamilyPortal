using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class SerializableUser
    {
        [MaxLength(30)]
        public string FirstName { get; set; }

        [MaxLength(40)]
        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [JsonIgnore]
        public string DisplayName => $"{FirstName} {LastName}".Trim();

        [MaxLength(40)]
        public string Title { get; set; }

        public string Bio { get; set; }

        public bool LocalAccount { get; set; }

        public UnderwriterGoal Goals { get; set; }

        public IEnumerable<SocialLink> SocialLinks { get; set; }
    }
}
