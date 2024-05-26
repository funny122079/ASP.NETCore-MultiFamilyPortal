using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.CoreUI
{
    public partial class FileIcon
    {
        [Parameter]
        public FileIconType FileType { get; set; }

        [Parameter]
        public string FileName { get; set; }

        private string _icon;
        private string _color;
        protected override void OnParametersSet()
        {
            FileTypeInfo fileInfo = null;
            if (!string.IsNullOrEmpty(FileName))
            {
                fileInfo = FileTypeLookup.GetFileTypeInfo(FileName);
            }
            else
            {
                var fileName = FileType switch
                {
                    FileIconType.Excel => "file.xlsx",
                    FileIconType.Word => "file.docx",
                    FileIconType.PDF => "file.pdf",
                    FileIconType.Image => "file.png",
                    FileIconType.Zip => "file.zip",
                    _ => "file.unknown"
                };
                if (fileName == "file.unknown")
                    return;

                fileInfo = FileTypeLookup.GetFileTypeInfo(fileName);
            }

            if (_icon != fileInfo?.Icon)
                _icon = fileInfo?.Icon;

            if (_color != fileInfo?.Color)
                _color = fileInfo?.Color;
        }
    }
}
