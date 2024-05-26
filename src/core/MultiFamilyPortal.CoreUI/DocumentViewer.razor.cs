using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MultiFamilyPortal.CoreUI
{
    public partial class DocumentViewer : IAsyncDisposable
    {
        [Parameter]
        public DocumentType Type { get; set; }

        [Parameter]
        public string Link { get; set; }

        [Inject]
        private IJSRuntime _jsRuntime { get; set; }

        private ElementReference _wrapperRef;
        private DotNetObjectReference<DocumentViewer> _currentRazorComponent;
        private readonly string _widgetId = Guid.NewGuid().ToString();
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && Type == DocumentType.PDF)
            {
                if (_currentRazorComponent == null)
                {
                    _currentRazorComponent = DotNetObjectReference.Create(this);
                }

                await _jsRuntime.InvokeVoidAsync("MFPortal.KendoInitialize", _wrapperRef, _widgetId, _currentRazorComponent);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if(Type == DocumentType.Image)
              return;

            await _jsRuntime.InvokeVoidAsync("MFPortal.DisposeKendo", _wrapperRef);
            if (_currentRazorComponent != null)
            {
                _currentRazorComponent.Dispose();
            }
        }
    }
}