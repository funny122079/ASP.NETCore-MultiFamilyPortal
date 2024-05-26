using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.CoreUI
{
    public partial class AuthorizedRole
    {
        [Parameter]
        public string Role { get; set; }

        [Parameter]
        public IEnumerable<string> Roles { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private string GetRoles()
        {
            if (!string.IsNullOrEmpty(Role))
                return Role;
            else if (Roles is null || !Roles.Any())
                throw new Exception("No Role or Roles were set");

            return string.Join(",", Roles);
        }
    }
}
