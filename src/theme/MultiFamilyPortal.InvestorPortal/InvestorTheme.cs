using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.InvestorPortal
{
    public class InvestorTheme : SideBarTheme.SideBarTheme, IPortalInvestorTheme
    {
        public override string Name => "Investor Theme";
        public override Type SideBar => typeof(Layouts.Menu);
    }
}
