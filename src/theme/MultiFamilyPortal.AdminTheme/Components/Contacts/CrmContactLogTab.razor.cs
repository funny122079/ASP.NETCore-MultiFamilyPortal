using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts
{
    public partial class CrmContactLogTab
    {
        [Parameter]
        public CRMContact Contact { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal User { get; set; }

        private CRMContactLog _addLog;
        private CRMContactLog _editLog;

        private readonly ObservableRangeCollection<CRMContactLog> _logs = new ObservableRangeCollection<CRMContactLog>();

        protected override void OnInitialized()
        {
            if(Contact.Logs is null)
            {
                Contact.Logs = new List<CRMContactLog>();
            }

            if(Contact.Logs.Any())
            {
                _logs.ReplaceRange(Contact.Logs);
            }
        }

        private void OnAdd()
        {
            _addLog = new CRMContactLog
            {
                ContactId = Contact.Id,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            };
        }

        private void OnEdit(GridCommandEventArgs args)
        {
            _editLog = args.Item as CRMContactLog;
        }

        private void OnSaveNewLog(CRMContactLog log)
        {
            if(log is not null)
            {
                Contact.Logs.Add(log);
            }

            _addLog = null;

            _logs.ReplaceRange(Contact.Logs);
        }

        private void OnUpdateLog(CRMContactLog log)
        {
            _editLog = null;

            _logs.ReplaceRange(Contact.Logs);
        }
    }
}
