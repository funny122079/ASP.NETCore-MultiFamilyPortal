using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.Pages
{
    public class LogoutModel : PageModel
    {
        private SignInManager<SiteUser> _signInManager { get; }

        public LogoutModel(SignInManager<SiteUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGet()
        {
            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
            }

            return Redirect("~/");
        }
    }
}
