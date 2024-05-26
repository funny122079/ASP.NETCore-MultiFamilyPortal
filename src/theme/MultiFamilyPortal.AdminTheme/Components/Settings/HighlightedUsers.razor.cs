using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Collections;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Settings
{
    public partial class HighlightedUsers
    {
        private readonly ObservableRangeCollection<HighlightableUser> _selectable = new();
        private readonly ObservableRangeCollection<HighlightableUser> _users = new();
        private string _addUser;

        [Inject]
        private HttpClient _client { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var response = await _client.GetFromJsonAsync<IEnumerable<HighlightableUser>>("/api/admin/settings/highlightable-users");

            _users.ReplaceRange(response.Where(x => x.Order > 0).OrderBy(x => x.Order));
            _selectable.ReplaceRange(response.Where(x => x.Order == 0).OrderBy(x => x.DisplayName));
        }

        private async Task OnUserAdded()
        {
            var user = _selectable.FirstOrDefault(x => x.UserId == _addUser);
            if (user is null)
                return;

            _users.Add(user);
            await SaveState();
        }

        private async Task OnDeleteUser(GridCommandEventArgs args)
        {
            var user = args.Item as HighlightableUser;
            _users.Remove(user);
            await SaveState();
        }

        private async Task OnRowDropHandler(GridRowDropEventArgs<HighlightableUser> args)
        {
            //The data manipulations in this example are to showcase a basic scenario.
            //In your application you should implement them as per the needs of the project.

            _users.Remove(args.Item);

            var destinationItemIndex = _users.IndexOf(args.DestinationItem);

            if (args.DropPosition == GridRowDropPosition.After)
            {
                destinationItemIndex++;
            }

            _users.Insert(destinationItemIndex, args.Item);

            await SaveState();
        }

        private async Task SaveState()
        {
            for(int i = 0; i < _users.Count; i++)
            {
                _users[i].Order = 1 + i;
            }

            using var response = await _client.PostAsJsonAsync("/api/admin/settings/highlightable-users/update", _users);

            if(response.IsSuccessStatusCode)
            {
                var updated = await response.Content.ReadFromJsonAsync<IEnumerable<HighlightableUser>>();
                _users.ReplaceRange(updated.Where(x => x.Order > 0).OrderBy(x => x.Order));
                _selectable.ReplaceRange(updated.Where(x => x.Order == 0).OrderBy(x => x.DisplayName));
            }
        }
    }
}
