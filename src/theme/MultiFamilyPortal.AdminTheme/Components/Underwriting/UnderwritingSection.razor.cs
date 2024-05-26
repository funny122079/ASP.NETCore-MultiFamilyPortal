using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Extensions;
using Telerik.Blazor.Components;
using Telerik.DataSource;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingSection
    {
        private PortalNotification notification { get; set; }

        [Required]
        [Parameter]
        public UnderwritingColumn Column { get; set; }

        [Required]
        [Parameter]
        public UnderwritingType Type { get; set; }

        [Required]
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public RenderFragment TopLevelMenu { get; set; }

        private TelerikGrid<UnderwritingAnalysisLineItem> grid { get; set; }

        private UnderwritingAnalysisLineItem NewItem;
        private UnderwritingAnalysisLineItem EditItem;
        private IEnumerable<UnderwritingAnalysisLineItem> Items
        {
            get
            {
                if (Property is null)
                    return Array.Empty<UnderwritingAnalysisLineItem>();

                if(Column == UnderwritingColumn.Sellers)
                {
                    if (Type == UnderwritingType.Income)
                        return Property.SellerIncome;

                    return Property.SellerExpense;
                }

                if (Type == UnderwritingType.Income)
                    return Property.OurIncome;

                return Property.OurExpense;
            }
        }

        private IEnumerable<ExpenseSheetType> ExpenseTypes => Enum.GetValues<ExpenseSheetType>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                var desiredState = new GridState<UnderwritingAnalysisLineItem>
                {
                    GroupDescriptors = new List<GroupDescriptor> {
                        new GroupDescriptor {
                            Member = nameof(UnderwritingAnalysisLineItem.Category),
                            MemberType = typeof(UnderwritingCategory),
                        }
                    }
                };
                await grid.SetState(desiredState);
            }
        }

        private IEnumerable<UnderwritingCategory> AllowableCategories()
        {
            if (Column == UnderwritingColumn.Ours)
            {
                if (Type == UnderwritingType.Income)
                    return IncomeCategories.Where(x => x != UnderwritingCategory.PhysicalVacancy);
                return ExpenseCategories.Where(x => x != UnderwritingCategory.Management);
            }

            if (Type == UnderwritingType.Income)
                return IncomeCategories;

            return ExpenseCategories;
        }

        private void OnCreate()
        {
            NewItem = new UnderwritingAnalysisLineItem()
            {
                Category = Type == UnderwritingType.Income ? UnderwritingCategory.GrossScheduledRent : UnderwritingCategory.Taxes,
                ExpenseType = ExpenseSheetType.T12
            };
        }
        private void SaveNewLineItemWithClose()
        {
            SaveNewLineItem();
            NewItem = null;
        }

        private void SaveNewLineItem()
        {
            if (NewItem is null)
                return;
            if (string.IsNullOrEmpty(NewItem.Description))
                NewItem.Description = NewItem.Category.GetDisplayName();

            NewItem.Id = Guid.NewGuid();

            if(Column == UnderwritingColumn.Sellers)
            {
                Property.AddSellerItem(NewItem);
            }
            else
            {
                Property.AddOurItem(NewItem);
            }

            notification.ShowSuccess($"{Type} line item {NewItem.Category.GetDisplayName()}");
            NewItem = new();
        }

        private void ShowEditDialog(GridCommandEventArgs args)
        {
            EditItem = args.Item as UnderwritingAnalysisLineItem;
        }

        private void SaveEditLineItem()
        {
            if (EditItem is null)
                return;
            if (string.IsNullOrEmpty(EditItem.Description))
                EditItem.Description = EditItem.Category.GetDisplayName();
            //Items.Add(EditItem);

            notification.ShowSuccess($"{Type} line item {EditItem.ExpenseType.GetDisplayName()}");
            EditItem = null;
        }

        private void OnDelete(GridCommandEventArgs args)
        {
            var item = args.Item as UnderwritingAnalysisLineItem;

            if(Column == UnderwritingColumn.Sellers)
            {
                Property.RemoveSellerItem(item);
            }
            else
            {
                Property.RemoveOurItem(item);
            }
        }

        private double CalculatePercent(UnderwritingAnalysisLineItem item)
        {
            var allItems = Column == UnderwritingColumn.Sellers ? Property.Sellers : Property.Ours;
            var gsr = allItems.FirstOrDefault(x => x.Category == UnderwritingCategory.GrossScheduledRent);
            if (gsr is null)
                return 0;

            var percent = item.AnnualizedTotal / gsr.AnnualizedTotal;
            return percent;
        }

        private static readonly IEnumerable<UnderwritingCategory> IncomeCategories =
            Enum.GetValues<UnderwritingCategory>().Where(x => x.GetLineItemType() == UnderwritingType.Income);

        private static readonly IEnumerable<UnderwritingCategory> ExpenseCategories =
            Enum.GetValues<UnderwritingCategory>().Where(x => x.GetLineItemType() == UnderwritingType.Expense);
    }
}
