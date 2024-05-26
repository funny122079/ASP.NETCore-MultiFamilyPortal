using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Closing
{
    public partial class UnderwritingMortgagesTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private ObservableRangeCollection<UnderwritingAnalysisMortgage> Mortgages = new ObservableRangeCollection<UnderwritingAnalysisMortgage>();

        private UnderwritingAnalysisMortgage AddMortgage;
        private UnderwritingAnalysisMortgage EditMortgage;
        private bool _editable;
        protected override void OnInitialized()
        {
            _editable = _user.IsAuthorizedInPolicy(PortalPolicy.Underwriter);
            Mortgages.ReplaceRange(Property.Mortgages);
        }

        private void OnCreate()
        {
            AddMortgage = new UnderwritingAnalysisMortgage {
                //PropertyId = Property.Id,
                LoanAmount = Property.Mortgages.Any() ? 0 : Property.PurchasePrice * 0.8,
                TermInYears = 30,
                InterestRate = 0.04,
                Points = 0.01,
            };
        }

        private void OnSaveNewMortage(UnderwritingAnalysisMortgage mortgage)
        {
            Property.AddMortgage(mortgage);
            //Property.Mortgages.Add(mortgage);
            //DbContext.UnderwritingMortgages.Add(mortgage);
            Mortgages.Add(mortgage);
            //await DbContext.SaveChangesAsync();
            //Property.Update();
            AddMortgage = null;
        }

        private void OnEdit(GridCommandEventArgs args)
        {
            EditMortgage = args.Item as UnderwritingAnalysisMortgage;
        }

        private void OnUpdateMortgage(UnderwritingAnalysisMortgage mortgage)
        {
            //DbContext.UnderwritingMortgages.Update(mortgage);
            //await DbContext.SaveChangesAsync();
            //Property.Update();
            EditMortgage = null;
        }

        private void OnDelete(GridCommandEventArgs args)
        {
            var item = args.Item as UnderwritingAnalysisMortgage;

            Property.RemoveMortgage(item);
            //Property.Mortgages.Remove(item);
            //DbContext.UnderwritingMortgages.Remove(item);
            Mortgages.Remove(item);
            //await DbContext.SaveChangesAsync();
        }
    }
}
