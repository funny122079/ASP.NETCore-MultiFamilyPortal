using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace MultiFamilyPortal.CoreUI.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static bool TryGetQueryString(this NavigationManager navManager, string key, out string value)
        {
            var uri = navManager.ToAbsoluteUri(navManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString))
            {
                value = valueFromQueryString;
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryGetQueryString<T>(this NavigationManager navManager, string key, out T value)
        {
            if(navManager.TryGetQueryString(key, out string valueAsString))
            {
                try
                {
                    value = (T)Convert.ChangeType(valueAsString, typeof(T));
                    return true;
                }
                catch (Exception)
                {
                }
            }

            value = default;
            return false;
        }
    }
}
