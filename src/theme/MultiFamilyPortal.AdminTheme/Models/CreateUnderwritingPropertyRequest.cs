namespace MultiFamilyPortal.AdminTheme.Models
{
    public class CreateUnderwritingPropertyRequest
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Market { get; set; }

        public int Vintage { get; set; }

        public int Units { get; set; }
    }
}
