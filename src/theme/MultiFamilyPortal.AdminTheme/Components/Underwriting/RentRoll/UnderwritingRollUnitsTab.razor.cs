using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.RentRoll
{
    public partial class UnderwritingRollUnitsTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private IEnumerable<UnderwritingAnalysisModel> _allFloors = Array.Empty<UnderwritingAnalysisModel>();
        private readonly ObservableRangeCollection<DisplayUnit> _filteredUnits = new ();
        private IEnumerable<DisplayUnit> _allUnits;
        private DisplayUnit _unit;
        private string _query;
        private bool _showAddUnit = false;
        private bool _editable;
        private DisplayUnit _newUnit = null;
        private UnderwritingAnalysisModel _floor = null;

        protected override void OnInitialized()
        {
            _editable = _user.IsAuthorizedInPolicy(PortalPolicy.Underwriter);
            UpdateRentRoll();
        }

        private void UpdateRentRoll()
        {
            if(!Property.Models.SelectMany(x => x.Units).Any())
            {
                _allUnits = Array.Empty<DisplayUnit>();
                _filteredUnits.Clear();
                return;
            }

            _allUnits = Property.Models.SelectMany(m => m.Units.Select(u => new DisplayUnit(u, m)));

            var filtered = _allUnits;
            if (!string.IsNullOrEmpty(_query))
                filtered = _allUnits.Where(x =>
                    x.FloorPlanName.Contains(_query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.UnitName.Contains(_query, StringComparison.CurrentCultureIgnoreCase) ||
                    x.Renter.Contains(_query, StringComparison.CurrentCultureIgnoreCase));

            _filteredUnits.ReplaceRange(filtered.OrderBy(x => x.FloorPlanName).ThenBy(x => x.UnitName));
        }

        private void ListAllModels() => _allFloors = Property.Models.Where(x => x.TotalUnits > x.Units.Count());
        
        private void UpdateUnit(GridCommandEventArgs args) => _unit = args.Item as DisplayUnit;

        private void OnAddFloorUnits()
        {
            ListAllModels();
            _showAddUnit = true;
        }

        private void ChooseFloor(ChangeEventArgs args)
        {
            if(string.IsNullOrEmpty(args.Value.ToString()))
            {
                _newUnit = null;
                return;
            }

            _floor = _allFloors.FirstOrDefault(x => x.Id.ToString() == args.Value.ToString());
            _newUnit = new DisplayUnit(_floor);
        }

        private void RemoveUnit(UnderwritingAnalysisUnit unit)
        {
            var model = Property.Models.FirstOrDefault(m => m.Units.Contains(unit));
            model?.Units.Remove(unit);
        }

        private void HideWindow(DisplayUnit unit)
        {
            UpdateRentRoll();
            _showAddUnit = false;
            _newUnit = unit;
        }

        private void RefreshFloors()
        {
            UpdateRentRoll();
            _newUnit = new DisplayUnit(_floor);
        }
    }
}
