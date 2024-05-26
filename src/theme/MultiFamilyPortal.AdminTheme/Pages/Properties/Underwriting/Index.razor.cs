using System.Net.Http.Json;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Components.Underwriting;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.AdminTheme.Pages.Properties.Underwriting
{
    [Authorize(Policy = PortalPolicy.UnderwritingViewer)]
    public partial class Index
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILoggerFactory _loggerFactory { get; set; }

        [Inject]
        private ITimeZoneService _timezone { get; set; }

        private UnderwritingList underwritingList { get; set; }
        private UnderwriterResponse Profile;
        private string ProfileId;
        private string Status;
        private DateTimeOffset Start = DateTimeOffset.Now.AddYears(-1);
        private DateTimeOffset End = DateTimeOffset.Now;
        private ObservableRangeCollection<UnderwriterResponse> Underwriters = new ObservableRangeCollection<UnderwriterResponse> { new UnderwriterResponse { DisplayName = "All" } };
        private IEnumerable<string> AvailableStatus = Enum.GetValues<UnderwritingStatus>().AsEnumerable().Select(x => x.Humanize(LetterCasing.Title));
        
        protected override async Task OnInitializedAsync()
        {
            Profile = Underwriters.First();
            ProfileId = Profile.Id;
            var list = AvailableStatus.ToList();
            list.Insert(0, "All");
            AvailableStatus = list;
            Status = AvailableStatus.FirstOrDefault();
            
            Start = await _timezone.GetLocalDateTime(Start);
            End = await _timezone.GetLocalDateTime(End);

            await GetUnderwriters();
            await UpdateAsync();
        }

        private async Task GetUnderwriters()
        {
            try
            {
                var underwriters = await _client.GetFromJsonAsync<IEnumerable<UnderwriterResponse>>("/api/admin/underwriting/underwriters");
                Underwriters.AddRange(underwriters);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("Underwriting");
                logger.LogError(ex, "Error while trying to get underwriters");
            }
        }

        private async Task UpdateAsync()
        {
            Profile = Underwriters.First(x => x.Id == ProfileId);
            await underwritingList.Update();
        }
    }
}
