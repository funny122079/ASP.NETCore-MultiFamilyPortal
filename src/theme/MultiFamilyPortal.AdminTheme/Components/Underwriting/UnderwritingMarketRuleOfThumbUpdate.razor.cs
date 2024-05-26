using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingMarketRuleOfThumbUpdate
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public UnderwritingGuidance Guidance { get; set; }

        [Parameter]
        public EventCallback<double> UpdateCurrent { get; set; }

        [Parameter]
        public bool Show { get; set; }

        private double _currentCost;
        private double _intialCost;
        private double _min;
        private double _max;
        private double _smallStep;
        private double _largeStep;

        protected override void OnParametersSet() => VerifyInfo();

        private void VerifyInfo()
        {
            if (Guidance == null)
                return;

            if (Guidance.Type == CostType.PercentOfPurchase)
            {
                _min = Guidance.Min * Property.PurchasePrice / 2;
                _max = Guidance.Max * Property.PurchasePrice * 2;
            }
            else if (Guidance.Type == CostType.PerDoor)
            {
                _min = Guidance.Min * Property.Units / 2;
                _max = Guidance.Max * Property.Units * 2;
            }
            else
            {
                _min = Guidance.Min / 2;
                _max = Guidance.Max * 2;
            }

            var propertyExpenses = Property.Ours.Where(x => x.Category == Guidance.Category);
            _intialCost = propertyExpenses.Any() ? propertyExpenses.Sum(x => x.AnnualizedTotal) : _min;

            if (_intialCost < _min)
                _min *= 0.7;
            if (_intialCost > _max)
                _max = _intialCost * 1.3;

            int numberOfMajorPoints = 6;
            int numberOfMinorPoints = 7;
            _currentCost = _intialCost;

            _smallStep = Math.Round(Math.Floor((_max - _min) / numberOfMajorPoints), 2);
            _largeStep = Math.Round(Math.Floor((_max - _min) / numberOfMinorPoints), 2);
        }

        private async Task InnerUpdate() => await UpdateCurrent.InvokeAsync(_currentCost);
    }
}
