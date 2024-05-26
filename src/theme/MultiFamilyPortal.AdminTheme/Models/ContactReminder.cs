using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class ContactReminder
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool SystemGenerated { get; set; }
        public bool Dismissed { get; set; }
        public CRMContact Contact { get; set; }
    }
}
