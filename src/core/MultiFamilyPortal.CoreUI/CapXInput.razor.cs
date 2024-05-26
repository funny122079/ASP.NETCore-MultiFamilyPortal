using System.Text;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.CoreUI
{
    public partial class CapXInput
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public bool Enabled { get; set; } = true;

        [Parameter]
        public string Id { get; set; }

        protected override void OnInitialized()
        {
            _capx = Property.CapX.ToString(Format());
            CostType = Property.CapXType;
        }

        private string _capx;
        private readonly CostType[] _costTypes = Enum.GetValues<CostType>();

        private void FormatToDecimal()
        {
            var sanitized = GetSanitizedInput();
            if (double.TryParse(sanitized, out var value))
            {
                if(CostType == CostType.PercentOfPurchase)
                {
                    while (value > 1)
                        value /= 100;
                }

                Property.CapX = value;
            }

            _capx = Property.CapX.ToString(Format());
        }

        private void FormatToNumber()
        {
            var sanitized = GetSanitizedInput();
            if(double.TryParse(sanitized, out var value))
            {
                var format = "N";
                if (CostType == CostType.PercentOfPurchase)
                {
                    value /= 100;
                    format = "N4";
                }

                _capx = value.ToString(format);
            }
        }

        private CostType CostType
        {
            get => Property.CapXType;
            set => ProcessCapXType(value, Property.CapXType);
        }

        private void ProcessCapXType(CostType newValue, CostType oldvalue)
        {
            if(newValue != oldvalue)
            {
                Property.CapXType = newValue;
                Property.CapX = newValue switch
                {
                    CostType.PerDoor when oldvalue == CostType.Total => Property.CapX / Property.Units,
                    CostType.PerDoor when oldvalue == CostType.PercentOfPurchase => (Property.CapX * Property.PurchasePrice) / Property.Units,
                    CostType.Total when oldvalue == CostType.PercentOfPurchase => Property.CapX * Property.PurchasePrice,
                    CostType.Total when oldvalue == CostType.PerDoor => Property.CapX * Property.Units,
                    CostType.PercentOfPurchase when oldvalue == CostType.Total => Property.CapX / Property.PurchasePrice,
                    CostType.PercentOfPurchase when oldvalue == CostType.PerDoor => (Property.CapX * Property.Units) / Property.PurchasePrice,
                    _ => Property.CapX
                };

                _capx = Property.CapX.ToString(Format());
            }
        }

        private string GetSanitizedInput()
        {
            var trimmed = _capx.Trim();
            if (string.IsNullOrEmpty(trimmed))
                return "0";

            var sb = new StringBuilder();
            for(var i = 0; i < trimmed.Length; i++)
            {
                var c = trimmed[i];
                if(char.IsDigit(c) || c == '.')
                    sb.Append(c);
            }

            return sb.ToString();
        }

        private string Format() => CostType switch
        {
            CostType.PercentOfPurchase => "P2",
            _ => "C"
        };
    }
}
