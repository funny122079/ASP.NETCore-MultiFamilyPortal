using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactPhoneList
    {
        [Parameter]
        public CRMContact Contact { get; set; }

        private PortalNotification notification;

        private readonly IEnumerable<CRMContactPhoneType> _phoneTypes = Enum.GetValues<CRMContactPhoneType>();
        private CRMContactPhone _newPhone = new CRMContactPhone();

        private void AddPhone()
        {
            var phone = new CRMContactPhone
            {
                ContactId = Contact.Id,
                Number = _newPhone.Number.Trim(),
                Type = _newPhone.Type
            };
            Contact.Phones.Add(phone);
            _newPhone = new CRMContactPhone();
        }

        private async Task SetPrimaryPhone(CRMContactPhone phone)
        {
            var phones = Contact.Phones.ToArray();
            Contact.Phones = new List<CRMContactPhone>();
            foreach(var record in phones)
            {
                record.Primary = record.Number == phone.Number;
            }
            await Task.Delay(1);
            Contact.Phones = new List<CRMContactPhone>(phones);
        }

        private string GetPrimaryBadge(CRMContactPhone phone) =>
            phone.Primary ? "fa-badge-check" : "fa-badge";

        private string GetPrimaryBadgeColor(CRMContactPhone phone) =>
            phone.Primary ? "color: var(--bs-primary);" : string.Empty;

        private void OnDeletePhone(CRMContactPhone phone)
        {
            Contact.Phones.Remove(phone);
        }

        private bool CanAddPhone()
        {
            if (string.IsNullOrEmpty(_newPhone?.Number) || _newPhone.Number.Trim().Length != 10 || Contact.Phones.Any(x => x.Number == _newPhone.Number))
                return false;

            return true;
        }
    }
}
