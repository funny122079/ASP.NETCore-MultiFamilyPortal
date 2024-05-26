namespace MultiFamilyPortal.AdminTheme.Models
{
    public class InvestorSummary
    {
        public Guid Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Contacted { get; set; }
    }
}
