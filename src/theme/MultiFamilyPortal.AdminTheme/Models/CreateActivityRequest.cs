using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class CreateActivityRequest
    {
        public TimeSpan Total { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string Notes { get; set; }

        public ActivityType Type { get; set; }
    }
}
