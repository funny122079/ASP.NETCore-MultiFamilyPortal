using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;
using Telerik.Blazor.Components;
using Telerik.DataSource.Extensions;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactNotableDatesTab
    {
        [Parameter]
        public CRMContact Contact { get; set; }

        private int _index = -1;

        protected override void OnInitialized()
        {
            if(Contact.NotableDates is null)
                Contact.NotableDates = new List<CRMNotableDate>();
        }

        private void OnMarketAdded(GridCommandEventArgs args)
        {
            Contact.NotableDates.Add(args.Item as CRMNotableDate);
        }

        private void OnMarketEditing(GridCommandEventArgs args)
        {
            _index = Contact.NotableDates.IndexOf(args.Item);
        }

        private void OnMarketUpdated(GridCommandEventArgs args)
        {
            var updated = args.Item as CRMNotableDate;
            var existing = Contact.NotableDates.ElementAt(_index);
            existing.Date = updated.Date;
            existing.Description = updated.Description;
            existing.DismissReminders = updated.DismissReminders;
            existing.Recurring = updated.Recurring;
            existing.Type = updated.Type;
            _index = -1;
        }

        private void OnMarketDeleted(GridCommandEventArgs args)
        {
            var index = Contact.NotableDates.IndexOf(args.Item);
            var item = Contact.NotableDates.ElementAt(index);
            Contact.NotableDates.Remove(item);
        }
    }
}
