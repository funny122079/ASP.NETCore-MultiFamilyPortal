using System.ComponentModel.DataAnnotations;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Models
{
    internal class DbSettings
    {
        private IEnumerable<Setting> _allSettings { get; }
        private List<string> _updated { get; }

        public DbSettings(IEnumerable<Setting> settings)
        {
            _allSettings = settings;
            _updated = new List<string>();
        }

        public string SiteTitle
        {
            get => Get(PortalSetting.SiteTitle);
            set => Set(PortalSetting.SiteTitle, value);
        }

        public string SenderEmailName
        {
            get => Get(PortalSetting.NotificationEmailFrom);
            set => Set(PortalSetting.NotificationEmailFrom, value);
        }

        [EmailAddress]
        public string SenderEmail
        {
            get => Get(PortalSetting.NotificationEmail);
            set => Set(PortalSetting.NotificationEmail, value);
        }

        public string BackgroundColor
        {
            get => Get(PortalSetting.BackgroundColor);
            set => Set(PortalSetting.BackgroundColor, value);
        }

        public string ThemeColor
        {
            get => Get(PortalSetting.ThemeColor);
            set => Set(PortalSetting.ThemeColor, value);
        }

        public string Twitter
        {
            get => Get(PortalSetting.SocialTwitter);
            set => Set(PortalSetting.SocialTwitter, value);
        }

        public string Facebook
        {
            get => Get(PortalSetting.SocialFacebook);
            set => Set(PortalSetting.SocialFacebook, value);
        }

        public string LinkedIn
        {
            get => Get(PortalSetting.SocialLinkedIn);
            set => Set(PortalSetting.SocialLinkedIn, value);
        }

        public string Instagram
        {
            get => Get(PortalSetting.SocialInstagram);
            set => Set(PortalSetting.SocialInstagram, value);
        }

        public string LegalName
        {
            get => Get(PortalSetting.LegalBusinessName);
            set => Set(PortalSetting.LegalBusinessName, value);
        }
        public string Address
        {
            get => Get(PortalSetting.ContactStreetAddress);
            set => Set(PortalSetting.ContactStreetAddress, value);
        }
        public string City
        {
            get => Get(PortalSetting.ContactCity);
            set => Set(PortalSetting.ContactCity, value);
        }
        public string State
        {
            get => Get(PortalSetting.ContactState);
            set => Set(PortalSetting.ContactState, value);
        }
        public string PostalCode
        {
            get => Get(PortalSetting.ContactZip);
            set => Set(PortalSetting.ContactZip, value);
        }

        [EmailAddress]
        public string PublicEmail
        {
            get => Get(PortalSetting.ContactEmail);
            set => Set(PortalSetting.ContactEmail, value);
        }

        public string Phone
        {
            get => Get(PortalSetting.ContactPhone);
            set => Set(PortalSetting.ContactPhone, value);
        }

        public int BlogPageLimit
        {
            get => Get<int>(PortalSetting.BlogPageLimit);
            set => Set(PortalSetting.BlogPageLimit, value);
        }

        public string BlogDefaultImage
        {
            get => Get(PortalSetting.ImageUrl);
            set => Set(PortalSetting.ImageUrl, value);
        }

        public string Webmaster
        {
            get => Get(PortalSetting.Webmaster);
            set => Set(PortalSetting.Webmaster, value); 
        }

        public string ManagingEditor
        {
            get => Get(PortalSetting.ManagingEditor);
            set => Set(PortalSetting.ManagingEditor, value);
        }

        public string RssDescription
        {
            get => Get(PortalSetting.RssDescription);
            set => Set(PortalSetting.RssDescription, value);
        }

        private string Get(string key)
        {
            return _allSettings.First(x => x.Key == key).Value;
        }

        private T Get<T>(string key) =>
            (T)Convert.ChangeType(Get(key), typeof(T));

        private void Set(string key, string value)
        {
            _allSettings.First(x => x.Key == key).Value = value;

            if(!_updated.Contains(key))
                _updated.Add(key);
        }

        private void Set<T>(string key, T value) =>
            Set(key, value.ToString());

        public IEnumerable<Setting> UpdatedSettings()
        {
            foreach (var key in _updated)
                yield return _allSettings.First(x => x.Key == key);
        }
    }
}
