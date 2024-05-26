using System.Text;
using UAParser;

namespace MultiFamilyPortal.Extensions
{
    public static class UserAgentExtensions
    {
        public static string Version(this UserAgent userAgent)
        {
            if (string.IsNullOrEmpty(userAgent.Major))
                return null;

            var sb = new StringBuilder();
            sb.Append(userAgent.Major);

            if (!string.IsNullOrEmpty(userAgent.Minor))
            {
                sb.Append($".{userAgent.Minor}");
                if (!string.IsNullOrEmpty(userAgent.Patch))
                {
                    sb.Append($".{userAgent.Patch}");
                }
            }

            return sb.ToString();
        }
    }
}
