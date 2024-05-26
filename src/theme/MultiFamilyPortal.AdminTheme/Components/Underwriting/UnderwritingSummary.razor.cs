using System.Reactive;
using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using ReactiveUI;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting;

public partial class UnderwritingSummary : IDisposable
{
    [Parameter]
    public UnderwritingAnalysis Property { get; set; }

    private ReactiveCommand<Unit, Unit> _refreshCommand;
    private CompositeDisposable _disposables = new();

    protected override void OnInitialized()
    {
        _refreshCommand = ReactiveCommand.CreateFromTask(Refresh).DisposeWith(_disposables);
        Property.WhenAnyValue(x => x.CapRate,
                x => x.DebtCoverage,
                x => x.CashOnCash,
                x => x.LTV,
                x => x.Reversion,
                x => x.NetPresentValue,
                x => x.InternalRateOfReturn,
                x => x.AnnualCashOnCashReturn,
                x => x.HoldYears,
                x => x.Vintage,
                x => x.Units,
                (cr, dc, coc, ltv, r, npv, irr, acocr, hy, v, u) => Unit.Default)
            .InvokeCommand(_refreshCommand)
            .DisposeWith(_disposables);
    }

    private ColorCode GetReversionColor()
    {
        if (Property.Reversion <= Property.PurchasePrice)
            return ColorCode.Danger;

        else if (Property.PurchasePrice <= 0)
            return ColorCode.Warning;

        var rate = Property.Reversion / Property.PurchasePrice;

        if (rate < 1.1)
            return ColorCode.Danger;
        else if (rate < 1.15)
            return ColorCode.Warning;

        return ColorCode.Success;
    }

    private ColorCode GetRaiseColor()
    {
        if (Property.PurchasePrice == 0 || Property.Raise == 0)
            return ColorCode.Info;

        var value = Property.Raise / Property.PurchasePrice;
        if (value < 0.35)
            return ColorCode.Info;
        else if (value < 0.45)
            return ColorCode.Warning;

        return ColorCode.Danger;
    }

    private ColorCode GetColor(double value, double danger, double warning)
    {
        if (value <= danger)
            return ColorCode.Danger;
        else if (value < warning)
            return ColorCode.Warning;

        return ColorCode.Success;
    }

    private ColorCode GetNPVColor()
    {
        if (Property.NetPresentValue >= 0)
            return ColorCode.Danger;

        return ColorCode.Success;
    }

    private async Task Refresh()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        if (!_disposables.IsDisposed)
            _disposables.Dispose();

        GC.SuppressFinalize(this);
    }
}
