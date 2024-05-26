using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;

namespace MultiFamilyPortal.Authentication
{
    internal class PortalClaimsTransformation : IClaimsTransformation
    {
        private IMFPContext _dbContext { get; }

        public PortalClaimsTransformation(IMFPContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user is null)
                return principal;

            AddOrReplaceClaim(ref principal, "preferred_username", user.DisplayName);
            AddOrReplaceClaim(ref principal, "name", user.DisplayName);
            AddOrReplaceClaim(ref principal, ClaimTypes.Name, user.DisplayName);
            AddOrReplaceClaim(ref principal, ClaimTypes.GivenName, user.FirstName);
            AddOrReplaceClaim(ref principal, ClaimTypes.Surname, user.LastName);

            return principal;
        }

        private void AddOrReplaceClaim(ref ClaimsPrincipal principal, string type, string newValue)
        {
            var claim = principal.Claims.FirstOrDefault(x => x.Type == type);

            var identity = principal.Identity as ClaimsIdentity;
            if (identity is null)
                return;

            if(claim is null)
            {
                identity.AddClaim(new Claim(type, newValue));
                return;
            }

            if(claim.Value != newValue)
            {
                identity.RemoveClaim(claim);
                identity.AddClaim(new Claim(type, newValue));
            }
        }
    }
}
