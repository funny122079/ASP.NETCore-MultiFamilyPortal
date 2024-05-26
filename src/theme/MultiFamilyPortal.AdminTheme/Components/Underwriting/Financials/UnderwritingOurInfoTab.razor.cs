using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Extensions;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Financials
{
    public partial class UnderwritingOurInfoTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public EventCallback ExpensesUpdated { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private bool _editable;

        private bool Refreshing;

        private readonly ObservableRangeCollection<UnderwritingAnalysisLineItem> Items = new ();

        protected override void OnInitialized()
        {
            _editable = _user.IsAuthorizedInPolicy(PortalPolicy.Underwriter);
        }

        protected override void OnParametersSet()
        {
            if (Property?.Ours?.Any() ?? false)
                Items.ReplaceRange(Property.Ours);
        }

        private async Task UpdateFromSeller()
        {
            Refreshing = true;

            var grouped = Property.Sellers.GroupBy(x => x.Category);

            var allCategories = Enum.GetValues<UnderwritingCategory>();
            var guidanceList = await _client.GetFromJsonAsync<IEnumerable<UnderwritingGuidance>>($"/api/admin/underwriting/guidance?market={Property.Market}");
            var updatedItems = new List<UnderwritingAnalysisLineItem>();
            foreach (var category in allCategories)
            {
                if (category == UnderwritingCategory.PhysicalVacancy)
                {
                    var vacancyRate = Math.Min(Property.PhysicalVacancy, Property.MarketVacancy);
                    if (vacancyRate < 0.05)
                        vacancyRate = 0.05;

                    var vacancy = new UnderwritingAnalysisLineItem
                    {
                        Amount = vacancyRate * Property.GrossPotentialRent,
                        Category = category,
                        Description = category.GetDisplayName(),
                        ExpenseType = ExpenseSheetType.T12
                    };
                    updatedItems.Add(vacancy);
                    continue;
                }
                else if (category == UnderwritingCategory.Management)
                {
                    var management = new UnderwritingAnalysisLineItem
                    {
                        Amount = Property.Management * Property.GrossPotentialRent,
                        Category = category,
                        Description = category.GetDisplayName(),
                        ExpenseType = ExpenseSheetType.T12
                    };
                    updatedItems.Add(management);
                    continue;
                }
                else if (category == UnderwritingCategory.OtherIncomeBad)
                {
                    if (!grouped.Any(x => x.Key == category))
                        continue;

                    var sum = grouped.First(x => x.Key == category)
                            .Sum(x => x.AnnualizedTotal);

                    if(sum > 0)
                    {
                        var badIncome = new UnderwritingAnalysisLineItem
                        {
                            Amount = sum / 2,
                            Category = category,
                            Description = $"{category.GetDisplayName()} (1/2 of SIP)",
                            ExpenseType = ExpenseSheetType.T12
                        };
                        updatedItems.Add(badIncome);
                    }

                    continue;
                }
                else if(category == UnderwritingCategory.OtherIncomeOneTime)
                {
                    continue;
                }

                var group = grouped.FirstOrDefault(x => x.Key == category);
                double total = 0;
                if(group != null)
                {
                    total = group.Select(x => x.AnnualizedTotal).Sum();
                }

                var item = new UnderwritingAnalysisLineItem {
                    Amount = total,
                    Category = category,
                    Description = category.GetDisplayName(),
                    ExpenseType = ExpenseSheetType.T12
                };

                var guidance = guidanceList.FirstOrDefault(x => x.Category == category);
                if (total == 0 && guidance != null)
                {
                    item.UpdateFromGuidance(guidance, Property);
                }

                updatedItems.Add(item);
            }

            Property.ReplaceOurItems(updatedItems);
            Items.ReplaceRange(updatedItems);
            await ExpensesUpdated.InvokeAsync();

            Refreshing = false;
        }
    }
}
