using Microsoft.SyndicationFeed;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.Extensions
{
    public static class SiteUserExtensions
    {
        public static SyndicationPerson Syndicate(this SiteUser author)
        {
            return new SyndicationPerson($"{author.FirstName} {author.LastName}".Trim(), author.Email);
        }
    }
}
