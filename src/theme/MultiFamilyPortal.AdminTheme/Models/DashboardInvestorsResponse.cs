using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class DashboardInvestorsResponse
    {
        public int Total { get; set; }
        public int Contacted { get; set; }
        public List<DashboardInvestor> Investors { get; set; }
    }
}
