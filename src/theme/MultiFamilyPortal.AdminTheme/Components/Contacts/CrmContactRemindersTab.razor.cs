using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactRemindersTab
    {
        [Parameter]
        public CRMContact Contact { get; set; }
        private List<CRMContactReminder> _reminders = new();
        private CRMContactReminder _newReminder = null;
        private CRMContactReminder _selectedReminder = null;
        private CRMContactReminder _tempReminder = null;
        private bool _confirmation = false;
        private int _pageSize = 10;
        private int _page = 1;
        protected override void OnParametersSet() => GetReminders();

        private void showReminder()
        {
            _newReminder = new CRMContactReminder
            {
                ContactId = Contact.Id,
                Date = DateTime.Now,
            };
        }

        private void HandleValidSubmit()
        {
            _newReminder.Description = _newReminder.Description.Trim();
            if (Contact.Reminders == null)
                Contact.Reminders = new List<CRMContactReminder> { _newReminder };
            else
                Contact.Reminders.Add(_newReminder);

            _newReminder = null;
            GetReminders();
        }

        private void EditLoop(CRMContactReminder reminder)
        {
            _selectedReminder = reminder;
            _tempReminder = new();
            _tempReminder.Date = _selectedReminder.Date;
            _tempReminder.Dismissed = _selectedReminder.Dismissed;
            _tempReminder.Description = _selectedReminder.Description;
        }

        private void HandleValidEdit()
        {
             _selectedReminder = null;
             GetReminders();
        }

        private void HandleOnCancel()
        {
            _selectedReminder.Date = _tempReminder.Date;
            _selectedReminder.Description = _tempReminder.Description;
            _selectedReminder.Dismissed = _tempReminder.Dismissed;
            _selectedReminder = null;
        }

        private void RemoveReminder()
        {
            Contact.Reminders.Remove(_selectedReminder);
            GetReminders();
            _selectedReminder = null;
        }

        private void OnSearch(ChangeEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Value.ToString()))
            {
                GetReminders();
                _reminders = _reminders.Where(r => r.Description.ToLower().Contains(args.Value.ToString().ToLower())).ToList();
            }
            else
            {
                GetReminders();
            }
        }

        private void KeyboardEventHandler(KeyboardEventArgs args)
        {
            if (args.Code == "Escape")
                GetReminders();
        }

        private void GetReminders()
        {
            _reminders = Contact.Reminders.Where(r => r.Date.Date >= DateTime.Now.Date  && r.Dismissed == false || r.Dismissed == false && r.Date.Date < DateTime.Now.Date ).OrderBy(x => x.Date).ToList();
        }
    }
}
