namespace MultiFamilyPortal.AdminTheme.Models
{
    public class DashboardUnderwritingResponse
    {
        public int MonthlyReports { get; set; }
        public int WeeklyReports { get; set; }
        public int WeeklyGoal { get; set; } = 0;
        public int Active { get; set; }
        public int Passed { get; set; }
        public int OfferSubmitted { get; set; }
        public int OfferAccepted { get; set; }
        public int OfferRejected { get; set; }
        public int LOISubmitted { get; set; }
        public int LOIAccepted { get; set; }
        public int LOIRejected { get; set; }
        public double MonthlyPercent { get; set; }
        public double WeeklyPercent { get; set; }
    }
}
