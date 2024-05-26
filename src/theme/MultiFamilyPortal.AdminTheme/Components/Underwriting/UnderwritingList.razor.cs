using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.AdminTheme.Models;
using Telerik.Blazor.Components;
using MultiFamilyPortal.Collections;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Data.Models;
using Humanizer;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingList
    {
        [Parameter]
        public UnderwriterResponse Profile { get; set; }

        [Parameter]
        public EventCallback<UnderwriterResponse> ProfileChanged { get; set; }

        [Parameter]
        public DateTimeOffset Start { get; set; }

        [Parameter]
        public EventCallback<DateTimeOffset> StartChanged { get; set; }

        [Parameter]
        public DateTimeOffset End { get; set; }

        [Parameter]
        public EventCallback<DateTimeOffset> EndChanged { get; set; }

        [Parameter]
        public string Status { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        public NavigationManager _navigationManager { get; set; }

        [Inject]
        private ILogger<UnderwritingList> Logger { get; set; }

        private CreateUnderwritingPropertyRequest NewProspect;
        private ObservableRangeCollection<ProspectPropertyResponse> Prospects = new();
        private ObservableRangeCollection<ProspectPropertyResponse> FilteredProspects = new();
        private PortalNotification notification { get; set; }
        private readonly ObservableRangeCollection<string> _markets = new();

        protected override async Task OnInitializedAsync()
        {
            await Update();
        }

        public async Task Update()
        {
            if (Start == default)
                Start = DateTimeOffset.Now.AddMonths(-1);

            if (End == default)
                End = DateTimeOffset.Now;

            try
            {
                var start = Start.ToQueryString();
                var end = End.ToQueryString();
                var underwriterId = Profile?.Id;
                var properties = await _client.GetFromJsonAsync<IEnumerable<ProspectPropertyResponse>>($"/api/admin/underwriting?start={start}&end={end}&underwriterId={underwriterId}");

                Prospects.ReplaceRange(properties);

                if (Status != "All")
                    FilteredProspects.ReplaceRange(Prospects.Where(x => x.Status == (UnderwritingStatus)Enum.Parse(typeof(UnderwritingStatus), Status.Dehumanize())));
                else
                    FilteredProspects.ReplaceRange(Prospects.Where(x => x.Status != UnderwritingStatus.Passed));
            }
            catch (Exception ex)
            {
                string message = "An error occurred while attempting to load the properties.";
                Logger.LogError($"{message} : {ex.Message}");
                notification.ShowError(message);
            }
        }

        private async Task CreateProperty()
        {
            NewProspect = new()
            {
                Vintage = 1900
            };

            try
            {
                _markets.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<string>>("/api/admin/underwriting/markets"));
            }
            catch (Exception ex)
            {
                notification.ShowError($"{ex.GetType().Name} - {ex.Message}");
            }
        }

        private async Task StartUnderwriting()
        {
            using var response = await _client.PostAsJsonAsync("/api/admin/underwriting/create", NewProspect);
            var property = await response.Content.ReadFromJsonAsync<ProspectPropertyResponse>();
            Prospects.Add(property);
            NavigateToProperty(property);
        }

        private void ViewProperty(GridCommandEventArgs args)
        {
            var property = args.Item as ProspectPropertyResponse;
            NavigateToProperty(property);
        }

        private void NavigateToProperty(ProspectPropertyResponse property)
        {
            _navigationManager.NavigateTo($"/admin/underwriting/property/{property.Id}");
        }
    }
}
