using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class UpdateActivityRequest
    {
        public Guid Id { get; set; }

        public TimeSpan Total { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string Notes { get; set; }

        public ActivityType Type { get; set; }
    }
}
