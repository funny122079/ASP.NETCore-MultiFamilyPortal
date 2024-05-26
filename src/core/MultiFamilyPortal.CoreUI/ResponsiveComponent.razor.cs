using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.CoreUI;

public partial class ResponsiveComponent : ComponentBase
{
    protected bool IsSmallScreen { get; set; }
    protected const string MediaQuery = "(max-width: 767px)";
}