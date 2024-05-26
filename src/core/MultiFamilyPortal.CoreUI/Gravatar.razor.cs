using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.CoreUI
{
    public partial class Gravatar
    {
        [Parameter]
        public string Email { get; set; }

        [Parameter]
        public int Size { get; set; } = 60;

        [Parameter]
        public string Alt { get; set; }

        [Parameter]
        public DefaultGravatar Default { get; set; } = DefaultGravatar.MysteryPerson;

        [Parameter]
        public string Css { get; set; } = "img img-fluid rounded-circle";

        public string GravatarUrl { get; set; }

        protected override void OnInitialized()
        {
            GravatarUrl = GravatarHelper.GetUri(Email, Size, Default);
        }
    }
}
