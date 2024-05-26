using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.CoreUI
{
    public partial class TimeSpanPicker
    {
        [Parameter]
        public TimeSpan TimeSpan { get; set; }

        [Parameter]
        public EventCallback<TimeSpan> TimeSpanChanged { get; set; }

        [Parameter]
        public string ClassName { get; set; }

        [Parameter]
        public string Id { get; set; }
        public DateTime min = new DateTime(1900, 1, 1, 0, 0, 0);
        public DateTime max = new DateTime(1900, 1, 1, 23, 59, 59);
        DateTime selectedTime { get; set; } = new DateTime(1900, 1, 1, 0, 0, 0);
        
        protected override void OnParametersSet()
        {
            selectedTime = new DateTime(1900, 1, 1, 0, 0, 0).Add(TimeSpan);
        }

        private async Task OnSelectedTimeChanged(DateTime selected)
        {
            TimeSpan = selected.TimeOfDay;
            await TimeSpanChanged.InvokeAsync(selected.TimeOfDay);
        }
    }
}