namespace MultiFamilyPortal.AdminTheme.Models
{
    public class DashboardInvestor
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Reason { get; set; }
        public string Timezone { get; set; }
        public double LookingToInvest { get; set; }
        public bool Contacted { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
