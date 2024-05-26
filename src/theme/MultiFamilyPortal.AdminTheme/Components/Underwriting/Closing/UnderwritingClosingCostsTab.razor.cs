using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Closing
{
    public partial class UnderwritingClosingCostsTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private bool _editable;
        private double _pointCost;
        private double _twoMonthReserves;
        private double _insurancePrepayment;

        protected override void OnInitialized()
        {
            _editable = _user.IsAuthorizedInPolicy(PortalPolicy.Underwriter);
            _pointCost = Property.Mortgages.Sum(x => x.PointCost);
            _twoMonthReserves = Property.Ours.Where(x => x.Category.IsOperatingExpense()).Sum(x => x.AnnualizedTotal / 6);
            _insurancePrepayment = Property.Ours.Where(x => x.Category == UnderwritingCategory.Insurance).Sum(x => x.AnnualizedTotal);
        }

        private string GetSubtotal()
        {
            return CalculateSubtotal().ToString("C");
        }

        private double CalculateSubtotal()
        {
            return Property.ClosingCosts + _pointCost + Property.AquisitionFee;
        }

        private string GetTotal()
        {
            var total = /*_twoMonthReserves + _insurancePrepayment +*/ CalculateSubtotal() + Property.DeferredMaintenance + Property.SECAttorney + Property.ClosingCostMiscellaneous + Property.ClosingCostOther;
            return total.ToString("C");
        }
    }
}
