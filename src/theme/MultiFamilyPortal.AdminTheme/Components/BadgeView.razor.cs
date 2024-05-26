using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.AdminTheme.Components
{
    public partial class BadgeView
    {
        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueDeleted { get; set; }

        private async Task Delete()
        {
            if (ValueDeleted.HasDelegate)
                await ValueDeleted.InvokeAsync(Value);
        }
    }
}
