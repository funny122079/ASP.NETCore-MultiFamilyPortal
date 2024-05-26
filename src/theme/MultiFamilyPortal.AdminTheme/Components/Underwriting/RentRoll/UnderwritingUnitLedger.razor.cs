using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.RentRoll
{
    public partial class UnderwritingUnitLedger
    {
        [Parameter]
        public DisplayUnit Unit { get; set; }

        private UnderwritingAnalysisUnitLedgerItem _editItem;
        private void CreateHandler(GridCommandEventArgs args)
        {
            Unit.Ledger.Add((UnderwritingAnalysisUnitLedgerItem)args.Item);
        }

        private void EditHandler(GridCommandEventArgs args)
        {
            _editItem = (UnderwritingAnalysisUnitLedgerItem)args.Item;
        }

        private void UpdateHandler(GridCommandEventArgs args)
        {
            var item = (UnderwritingAnalysisUnitLedgerItem)args.Item;
            _editItem.Rent = item.Rent;
            _editItem.ChargesCredits = item.ChargesCredits;
            _editItem.Type = item.Type;
        }

        private void DeleteHandler(GridCommandEventArgs args)
        {
            Unit.Ledger.Remove((UnderwritingAnalysisUnitLedgerItem)args.Item);
        }
    }
}
