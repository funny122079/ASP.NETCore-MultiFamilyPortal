using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.CoreUI
{
    public class PortalNotification : TelerikNotification
    {
        public PortalNotification()
        {
            AnimationType = AnimationType.SlideIn;
            VerticalPosition = NotificationVerticalPosition.Bottom;
            HorizontalPosition = NotificationHorizontalPosition.Center;
        }

        public void ShowSuccess(string message)
        {
            Show(new NotificationModel() {
                Text = message,
                ThemeColor = "success"
            });
        }

        public void ShowWarning(string message)
        {
            Show(new NotificationModel() {
                Text = message,
                ThemeColor = "warning"
            });
        }

        public void ShowError(string message)
        {
            Show(new NotificationModel() {
                Text = message,
                ThemeColor = "error",
            });
        }
    }
}
