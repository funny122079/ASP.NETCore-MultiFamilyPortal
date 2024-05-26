using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Dashboard
{
    public partial class ActivityChart
    {
        [Parameter]
        public DashboardActivityResponse UserActivity { get; set; }

        public IEnumerable<MyPieChartModel> _pieData = Array.Empty<MyPieChartModel>();

        private double _totalTime = 0;
        protected override void OnParametersSet()
        {
            var _piedata = new List<MyPieChartModel>();

            if (UserActivity.Breakdown is not null && UserActivity.Breakdown.Count > 0)
            {
                foreach (var item in UserActivity?.Breakdown)
                    _piedata.Add(new MyPieChartModel
                    {
                        SegmentName = item.Key.ToString(),
                        SegmentValue = item.Value
                    });


                _totalTime = _piedata.Sum(x => x.SegmentValue);
            }
            _pieData = _piedata;
        }

        public class MyPieChartModel
        {
            public string SegmentName { get; set; }
            public double SegmentValue { get; set; }
        }
    }
}