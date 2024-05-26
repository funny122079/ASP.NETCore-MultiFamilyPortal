using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class DisplayUnit
    {
        private UnderwritingAnalysisUnit _unit { get; }

        public DisplayUnit(UnderwritingAnalysisModel floorPlan)
        {
            FloorPlan = floorPlan;
            _unit = new UnderwritingAnalysisUnit()
            {
                Rent = floorPlan.CurrentRent,
                Ledger = new List<UnderwritingAnalysisUnitLedgerItem>()
            };
        }

        public DisplayUnit(UnderwritingAnalysisUnit unit, UnderwritingAnalysisModel floorPlan)
        {
            _unit = new UnderwritingAnalysisUnit
            {
                Id = unit.Id,
                AtWill = unit.AtWill,
                Balance = unit.Balance,
                DepositOnHand = unit.DepositOnHand,
                LeaseEnd = unit.LeaseEnd,
                LeaseStart = unit.LeaseStart,
                Ledger = new List<UnderwritingAnalysisUnitLedgerItem>(unit.Ledger.Select(x => new UnderwritingAnalysisUnitLedgerItem
                {
                    ChargesCredits = x.ChargesCredits,
                    Rent = x.Rent,
                    Type = x.Type
                })),
                Rent = unit.Rent,
                Renter = unit.Renter,
                Unit = unit.Unit
            };
            FloorPlan = floorPlan;
        }

        public UnderwritingAnalysisModel FloorPlan { get; set; }

        public string FloorPlanName => FloorPlan.Name;

        public Guid Id
        {
            get => _unit.Id;
            set => _unit.Id = value;
        }

        public string UnitName
        {
            get => _unit.Unit;
            set => _unit.Unit = value;
        }

        public string Renter
        {
            get => _unit.Renter;
            set => _unit.Renter = value;
        }

        public DateTime? LeaseStart
        {
            get => _unit.LeaseStart;
            set => _unit.LeaseStart = value;
        }

        public DateTime? LeaseEnd
        {
            get => _unit.LeaseEnd;
            set => _unit.LeaseEnd = value;
        }

        public bool AtWill
        {
            get => _unit.AtWill;
            set => _unit.AtWill = value;
        }

        public double Rent
        {
            get => _unit.Rent;
            set => _unit.Rent = value;
        }

        public double DepositOnHand
        {
            get => _unit.DepositOnHand;
            set => _unit.DepositOnHand = value;
        }

        public double Balance
        {
            get => _unit.Balance;
            set => _unit.Balance = value;
        }

        public List<UnderwritingAnalysisUnitLedgerItem> Ledger
        {
            get => _unit.Ledger;
            set => _unit.Ledger = value;
        }

        public void Add()
        {
            Id = Guid.NewGuid();
            if(!FloorPlan.Units?.Any(x => x.Unit.ToUpper().Trim() == _unit?.Unit?.ToUpper()?.Trim()) == true)
              FloorPlan.Units.Add(_unit);
        }

        public void Update()
        {
            if (Id == default)
                throw new InvalidOperationException("Cannot update a new Unit. This Unit has not been added to the Floor Plan Unit List");

            var unit = FloorPlan.Units.FirstOrDefault(x => x.Id == Id);
            if (unit is null)
                throw new InvalidOperationException("Cannot update this unit. Unit is not contained within the Floor Plan Unit List");

            unit.AtWill = AtWill;
            unit.Balance = Balance;
            unit.DepositOnHand = DepositOnHand;
            unit.LeaseEnd = LeaseEnd;
            unit.LeaseStart = LeaseStart;
            unit.Ledger = Ledger;
            unit.Rent = Rent;
            unit.Renter = Renter;
            unit.Unit = UnitName;
        }

        public void Remove()
        {
            if (Id == default)
                throw new InvalidOperationException("Cannot remove a new Unit. This Unit has not been added to the Floor Plan Unit List");

            var unit = FloorPlan.Units.FirstOrDefault(x => x.Id == Id);
            if (unit is null)
                return;

            FloorPlan.Units.Remove(unit);
        }
    }
}
