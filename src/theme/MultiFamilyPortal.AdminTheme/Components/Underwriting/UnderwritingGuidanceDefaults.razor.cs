using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;
using Telerik.Blazor.Components;
using Telerik.DataSource;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingGuidanceDefaults
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<UnderwritingGuidanceDefaults> _logger { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private bool _editable;
        private bool _editIntent;
        private readonly ObservableRangeCollection<UnderwritingGuidance> _guidance = new();
        private TelerikGrid<UnderwritingGuidance> grid;
        private bool _windowVisibility { get; set; }
        private string _marketTitle = null;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                _editable = _user.IsAuthorizedInPolicy(PortalPolicy.Underwriter);
                _guidance.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<UnderwritingGuidance>>("/api/admin/underwriting/guidance"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error populating Underwriting Guidance");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                var desiredState = new GridState<UnderwritingGuidance> {
                    GroupDescriptors = new[] {
                        new GroupDescriptor {
                            Member = nameof(UnderwritingGuidance.Market),
                            MemberType = typeof(string)
                        }
                    }
                };
                await grid.SetState(desiredState);
            }
        }

        private async Task RefreshGuidance()
        {
            await OnInitializedAsync();
            _windowVisibility = false;
            _marketTitle = null;
        }

        private void OnAddGuidance()
        {
            _marketTitle = null;
            _editIntent = false;
            _windowVisibility = true;
        }

        private void EditAddGuidance(string market)
        {
             _marketTitle = market;
             _editIntent = true;
             _windowVisibility = true;
        }
    }
}
