using Telerik.Blazor.Components;

namespace MultiFamilyPortal.CoreUI;

public class ResponsiveWindow : TelerikWindow
{
    public ResponsiveWindow()
    {
        Centered = true;
        Draggable = false;
        Modal = true;
        Class = "responsive-window";
    }
}
