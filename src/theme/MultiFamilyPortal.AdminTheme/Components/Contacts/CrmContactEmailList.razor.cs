using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using System.Net.Mail;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactEmailList
    {
        [Parameter]
        public CRMContact Contact { get; set; }

        private PortalNotification notification;
        private readonly IEnumerable<CRMContactEmailType> _emailTypes = Enum.GetValues<CRMContactEmailType>();
        private CRMContactEmail _newEMail = new CRMContactEmail();

        private void AddEmail()
        {
            var email = new CRMContactEmail
            {
                ContactId = Contact.Id,
                Email = _newEMail.Email.Trim().ToLowerInvariant(),
                Type = _newEMail.Type,
            };
            Contact.Emails.Add(email);
            _newEMail = new CRMContactEmail();
        }

        private async Task SetPrimaryEmail(CRMContactEmail email)
        {
            var emails = Contact.Emails.ToArray();
            Contact.Emails = new List<CRMContactEmail>();
            foreach (var record in emails)
            {
                record.Primary = record.Email == email.Email;
            }
            await Task.Delay(1);
            Contact.Emails = new List<CRMContactEmail>(emails);
        }

        private void OnDeleteEmail(CRMContactEmail email)
        {
            if (email is not null && Contact.Emails.Contains(email))
                Contact.Emails.Remove(email);
        }

        private bool CanAddEmail()
        {
            try
            {
                if (string.IsNullOrEmpty(_newEMail.Email) ||
                    Contact.Emails.Any(x => x.Email == _newEMail.Email.Trim().ToLowerInvariant()))
                    return false;

                _ = new MailAddress(_newEMail.Email);

                var domain = _newEMail.Email.Trim().Split('@')[1];
                var parts = domain.Split('.');
                var tld = parts.LastOrDefault();
                return parts.Length > 1 && tld.Length > 1;
            }
            catch
            {
                return false;
            }
        }
    }
}
