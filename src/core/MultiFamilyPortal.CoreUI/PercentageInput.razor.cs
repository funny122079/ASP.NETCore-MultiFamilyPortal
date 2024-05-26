using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.CoreUI
{
    public partial class PercentageInput
    {
        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public int Precision { get; set; } = 2;

        [Parameter]
        public double Value { get; set; }

        [Parameter]
        public double Min { get; set; } = 0;

        [Parameter]
        public double Max { get; set; } = 100;

        [Parameter]
        public bool Enabled { get; set; } = true;

        [Parameter]
        public EventCallback<double> ValueChanged { get; set; }

        private async Task HandleValueChangedAsync()
        {
            if (Value >= 1)
                Value /= 100;
            await ValueChanged.InvokeAsync(Value);
        }
    }
}
