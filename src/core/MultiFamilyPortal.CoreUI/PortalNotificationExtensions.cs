using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.CoreUI
{
    public static class PortalNotificationExtensions
    {
        public static void Show(this PortalNotification notification, FormResult result)
        {
            if (string.IsNullOrEmpty(result.Message))
                return;

            switch(result.State)
            {
                case ResultState.Success:
                    notification.ShowSuccess(result.Message);
                    break;
                case ResultState.Warning:
                    notification.ShowWarning(result.Message);
                    break;
                case ResultState.Error:
                    notification.ShowError(result.Message);
                    break;
            }
        }
    }
}
