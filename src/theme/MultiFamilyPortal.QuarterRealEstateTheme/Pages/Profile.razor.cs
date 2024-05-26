using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Pages
{
    public partial class Profile
    {
        private bool notFound;
        [Parameter]
        public string name { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private HighlightedUserResponse _user;
        protected override async Task OnInitializedAsync()
        {
            var parts = name.Split('-');
            if (parts.Length < 2)
            {
                notFound = true;
                return;
            }

            var firstName = parts[0].Trim();
            var sb = new StringBuilder();
            for (var i = 1; i < parts.Length; i++)
                sb.Append($"{parts[i]} ");
            var lastName = sb.ToString().Trim();
            var url = $"/api/about/profile/{firstName}/{lastName}";
            using var response = await _client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                notFound = true;
                return;
            }

            _user = await response.Content.ReadFromJsonAsync<HighlightedUserResponse>();
        }

        private string GetTitle()
        {
            if (notFound)
                return "Team Member Not Found";

            if (_user is null)
                return "Team Member";

            return _user.DisplayName;
        }
    }
}
