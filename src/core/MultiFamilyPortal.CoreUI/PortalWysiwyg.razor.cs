using Microsoft.AspNetCore.Components;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Editor;

namespace MultiFamilyPortal.CoreUI
{
    public partial class PortalWysiwyg
    {
        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public string Height { get; set; } = "500px";

        [Parameter]
        public string Width { get; set; } = "100%";

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string Class { get; set; }

        private readonly List<IEditorTool> _tools = new List<IEditorTool>
        {
            new EditorButtonGroup(
                new Bold(),
                new Italic(),
                new Underline()
            ),
            new Format(),
            new EditorButtonGroup(
                new AlignLeft(),
                new AlignCenter(),
                new AlignRight(),
                new AlignJustify()
            ),
            new EditorButtonGroup(
                new UnorderedList(),
                new OrderedList(),
                new Indent(),
                new Outdent()
            ),
            new EditorButtonGroup(
                new CreateLink(),
                new Unlink()
            ),
            new InsertImage(),
            new InsertTable(),
            new EditorButtonGroup(
                new Undo(),
                new Redo()
            ),
            new ViewHtml(),
        };

        private async Task OnValueChanged(string value)
        {
            if (Value == value)
                return;
            Value = value;
            await ValueChanged.InvokeAsync(value);
        }
    }
}
