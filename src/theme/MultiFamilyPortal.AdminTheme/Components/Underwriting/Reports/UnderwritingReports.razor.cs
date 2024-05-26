using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.SaaS.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Reports
{
    public partial class UnderwritingReports
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [CascadingParameter]
        private Tenant _tenant { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private string _reportName;
        private string _reportLink;
        private bool _comingSoon;

        private static readonly string[] _supported = new string[]
        {
            "manager-report",
            "investment-tiers",
            "cash-flow",
            "capital-expenses",
            "full-report",
            "deal-summary",
            "income-forecast",
            "assumptions",
        };

        private void SelectReport(string name)
        {
            _comingSoon = false;
            _reportName = null;

            if(!_supported.Contains(name))
            {
                _comingSoon = true;
                return;
            }

            _reportName = name;
            _reportLink = $"{NavigationManager.BaseUri}api/admin/reports/{_reportName}/{Property.Id}";
        }
    }
}
