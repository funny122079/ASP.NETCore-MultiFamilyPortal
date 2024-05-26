using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace MultiFamilyPortal.CoreUI
{
    public partial class PortalErrorBoundary
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Inject]
        private ILogger<PortalErrorBoundary> _logger { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private int _retryTimes = 0;
        private ErrorBoundary _errorBoundary;
        private void RecoverFromError()
        {
            _errorBoundary?.Recover();
        }

        private void HandleError(Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception caught by Error Boundary");

            if (_retryTimes > 3)
                ReportError(ex);

            _retryTimes++;
        }

        private void ReportError(Exception ex)
        {
            // TODO: Report error to your error reporting service #182
            _retryTimes = 0;
            _navigationManager.NavigateTo("/");
        }
    }
}
