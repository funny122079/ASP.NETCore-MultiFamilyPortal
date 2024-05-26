using System.Net.Http.Json;
using Humanizer;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.PropertyInfo
{
    public partial class UnderwritingFilesTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private ObservableRangeCollection<UnderwritingAnalysisFile> _files = new();
        private UnderwritingAnalysisFile _selectedFile;
        private readonly IEnumerable<string> fileTypes = Enum.GetValues<UnderwritingProspectFileType>()
            .Select(x => x.Humanize(LetterCasing.Title));
        private string selectedFileType = UnderwritingProspectFileType.OfferMemorandum.Humanize(LetterCasing.Title);
        private string description;
        private bool showUploadFile;
        private PortalNotification notification;

        protected override async Task OnInitializedAsync()
        {
            await Update();
        }

        private void CloseUpload()
        {
            showUploadFile = false;
            selectedFileType = UnderwritingProspectFileType.OfferMemorandum.Humanize(LetterCasing.Title);
            description = null;
        }

        private async Task OnFileUploaded()
        {
            CloseUpload();
            await Update();
        }

        private async Task Update()
        {
            var files = await _client.GetFromJsonAsync<IEnumerable<UnderwritingAnalysisFile>>($"/api/admin/underwriting/files/{Property.Id}");
            _files.ReplaceRange(files.OrderByDescending(x => x.Timestamp));
        }

        private async Task OnDelete(GridCommandEventArgs args)
        {
            var file = args.Item as UnderwritingAnalysisFile;
            if (file is null)
                return;

            using var result = await _client.DeleteAsync($"/api/admin/underwriting/files/{Property.Id}/file/{file.Id}");
            if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.NotFound)
                notification.ShowSuccess($"File '{file.Name}' has been successfully deleted.");

            await Update();
        }

        private void OnPreview(GridCommandEventArgs args)
        {
            _selectedFile = args.Item as UnderwritingAnalysisFile;
        }

        private UnderwritingProspectFileType GetFileType() =>
            Enum.TryParse<UnderwritingProspectFileType>(selectedFileType.Pascalize(), true, out var result)
            ? result : UnderwritingProspectFileType.OfferMemorandum;

        public string SaveUrl => ToAbsoluteUrl($"api/admin/underwriting/upload/save/{Property.Id}?fileType={GetFileType()}&description={description}");

        public string ToAbsoluteUrl(string url)
        {
            return $"{_navigationManager.BaseUri}{url}";
        }

        private void OnSelectHandler(UploadSelectEventArgs e)
        {
            if (string.IsNullOrEmpty(description))
                description = e.Files.FirstOrDefault().Name;
        }
    }
}
