using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class DashboardActivityResponse
    {
        public Dictionary<ActivityType, double> Breakdown { get; set; }
    }
}
