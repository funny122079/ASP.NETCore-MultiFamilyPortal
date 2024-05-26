using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;
using Telerik.DataSource.Extensions;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.PropertyInfo
{
    public partial class UnderwritingCapitalImprovementsTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private bool _editable;

        private int _index = -1;

        protected override void OnInitialized()
        {
            _editable = _user.IsAuthorizedInPolicy(PortalPolicy.Underwriter);
            if (Property.CapitalImprovements is null)
                Property.CapitalImprovements = new List<UnderwritingAnalysisCapitalImprovement>();
        }

        private void OnImprovementAdded(GridCommandEventArgs args)
        {
            Property.CapitalImprovements.Add(args.Item as UnderwritingAnalysisCapitalImprovement);
        }

        private void OnImprovementDeleted(GridCommandEventArgs args)
        {
            var index = Property.CapitalImprovements.IndexOf(args.Item);
            var item = Property.CapitalImprovements.ElementAt(index);
            Property.CapitalImprovements.Remove(item);
        }

        private void OnImprovementEditing(GridCommandEventArgs args)
        {
            _index = Property.CapitalImprovements.IndexOf(args.Item);
        }

        private void OnImprovementUpdated(GridCommandEventArgs args)
        {
            var updated = args.Item as UnderwritingAnalysisCapitalImprovement;
            var existing = Property.CapitalImprovements.ElementAt(_index);
            existing.Status = updated.Status;
            existing.Cost = updated.Cost;
            existing.Description = updated.Description;
            _index = -1;
        }
    }
}
