using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.AdminTheme.Components
{
    public partial class BadgeComponent
    {
        private string NewValue { get; set; }

        [Parameter]
        public IEnumerable<string> Values { get; set; } = default!;

        [Parameter]
        public string Placeholder { get; set; } = default!;

        [Parameter]
        public IEnumerable<string> Data { get; set; } = default!;

        [Parameter]
        public EventCallback<IEnumerable<string>> ValuesChanged { get; set; }

        private IEnumerable<string> AutocompleteData { get; set; }

        protected override void OnInitialized()
        {
            AutocompleteData = Data.Where(x => !Values.Contains(x));
        }

        private async Task OnValueChanged(object obj)
        {
            if (obj is null || !(obj is string str) || string.IsNullOrEmpty(str))
            {
                return;
            }
            else if (Values != null && Values.Contains(str))
            {
                NewValue = str = null;
                return;
            }

            var values = Values?.ToList() ?? new List<string>();
            values.Add(str);
            Values = values.Distinct().OrderBy(x => x);
            if (ValuesChanged.HasDelegate)
                await ValuesChanged.InvokeAsync(Values);
            AutocompleteData = Data.Where(x => !Values.Contains(x));
        }

        private async Task OnValueDeleted(string value)
        {
            if (Values != null && Values.Contains(value))
            {
                var values = Values.ToList();
                values.Remove(value);
                Values = values;
                if (ValuesChanged.HasDelegate)
                    await ValuesChanged.InvokeAsync(Values);
            }

            AutocompleteData = Data.Where(x => !Values.Contains(x));
        }
    }
}
