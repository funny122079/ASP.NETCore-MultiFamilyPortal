using System.Collections.Specialized;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Wizard
{
    public partial class RentRollWizardStep : IDisposable
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        protected override void OnInitialized()
        {
            if (Property.Models is INotifyCollectionChanged incc)
                incc.CollectionChanged += OnModelsUpdated;
        }

        private void OnModelsUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            StateHasChanged();
        }

        public void Dispose()
        {
            if (Property.Models is INotifyCollectionChanged incc)
                incc.CollectionChanged -= OnModelsUpdated;
        }
    }
}
