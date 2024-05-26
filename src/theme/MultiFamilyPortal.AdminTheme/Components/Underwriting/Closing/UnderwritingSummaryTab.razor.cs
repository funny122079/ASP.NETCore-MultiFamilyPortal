using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Closing
{
    public partial class UnderwritingSummaryTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private bool _editable;

        // Note: Seller notation simply refers to using the numbers provided by the seller
        // from the Sellers column in the Property. Our refers to the numbers from our underwriting
        private CostType[] _costTypes = new[] { CostType.PerDoor, CostType.Total };
        private double ourNOIAfterCapX => Property.NOI - Property.CapXTotal;
        private double sellerNOIAfterCapX => Property.SellerNOI - Property.CapXTotal;

        private double debtService;
        private double secondaryDebtService;

        private double ourCashFlow => Property.NOI - Property.CapXTotal - debtService - secondaryDebtService;
        private double sellerCashFlow => Property.SellerNOI - Property.CapXTotal - debtService - secondaryDebtService;
        private double ourEquityCF => ourCashFlow * Property.OurEquityOfCF;
        private double sellerEquityCF => sellerCashFlow * Property.OurEquityOfCF;

        private double ourEquityPartnerCF => ourCashFlow - ourEquityCF;
        private double sellerEquityPartnerCF => sellerCashFlow - sellerEquityCF;

        private double ourEquityPartnerCoC => ourEquityPartnerCF / Property.Raise;
        private double sellerEquityPartnerCoC => sellerEquityPartnerCF / Property.Raise;

        protected override void OnInitialized()
        {
            _editable = _user.IsAuthorizedInPolicy(PortalPolicy.Underwriter);
        }

        protected override void OnParametersSet()
        {
            if (Property?.Mortgages?.Any() ?? false)
            {
                debtService = Property.Mortgages
                    .Select(x => x.AnnualDebtService)
                    .OrderByDescending(x => x)
                    .FirstOrDefault();

                if (Property.Mortgages.Count() > 1)
                {
                    secondaryDebtService = Property.Mortgages
                        .Select(x => x.AnnualDebtService)
                        .Skip(1)
                        .Sum();
                }
            }
        }
    }
}
