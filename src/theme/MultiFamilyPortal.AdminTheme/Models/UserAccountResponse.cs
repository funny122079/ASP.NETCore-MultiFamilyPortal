namespace MultiFamilyPortal.AdminTheme.Models
{
    public class UserAccountResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string[] Roles { get; set; }
        public bool LocalAccount { get; set; }
    }
}
