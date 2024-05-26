using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Components
{
    public partial class InvestorTestimonial
    {
        [Parameter]
        public string InvestorPhoto { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Testimonial { get; set; }

        [Parameter]
        public int Stars { get; set; }
    }
}
