using Microsoft.AspNetCore.Authorization;

namespace MultiFamilyPortal.AdminTheme.Pages
{
    [Authorize(Roles = PortalRoles.PortalAdministrator)]
    public partial class Settings
    {
        
    }
}
