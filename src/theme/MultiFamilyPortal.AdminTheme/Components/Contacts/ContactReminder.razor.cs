using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Contacts;

public partial class ContactReminder
{
    [Parameter]
    public CRMContactReminder Reminder { get; set; }

}