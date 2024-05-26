using System.Reflection;
using System.Text.RegularExpressions;
using Humanizer;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Extensions;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace MultiFamilyPortal.AdminTheme.Services
{
    internal static class WorkSheetExtensions
    {
        private static Assembly Assembly = typeof(WorkSheetExtensions).Assembly;
        private const string CoachingForm = "Coaching Form";
        private const string UnderwritingNotes = "Underwriting Notes";

        public static Workbook LoadWorkbook(this XlsxFormatProvider formatProvider, string name)
        {
            var resourceId = Assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(name));
            return formatProvider.Import(Assembly.GetManifestResourceStream(resourceId));
        }

        public static void AddFiles(this Workbook workbook, IEnumerable<UnderwritingAnalysisFile> files)
        {
            if (files is null || !files.Any())
                return;

            var sheet = workbook.GetWorksheet("Files");
            for(int i = 0; i < files.Count(); i++)
            {
                var row = i + 3;
                var file = files.ElementAt(i);
                sheet.SetValue($"A{row}", file.Type)
                     .SetValue($"B{row}", file.Description)
                     .SetHyperlink($"C{row}", file.Name, file.DownloadLink);
            }
        }

        public static byte[] ExportAsByteArray(this XlsxFormatProvider formatProvider, Workbook workbook)
        {
            using var output = new MemoryStream();
            formatProvider.Export(workbook, output);
            return output.ToArray();
        }

        public static double Sum(this IEnumerable<UnderwritingAnalysisLineItem> lineItems, params UnderwritingCategory[] categories)
        {
            return lineItems.Where(x => categories.Any(c => c == x.Category))
                .Sum(x => x.AnnualizedTotal);
        }

        public static Worksheet GetWorksheet(this Workbook workbook, string name)
        {
            return workbook.Sheets.OfType<Worksheet>().FirstOrDefault(x => x.Name == name);
        }

        public static Worksheet SetHyperlink(this Worksheet sheet, string cellName, string displayName, string link)
        {
            var linkInfo = HyperlinkInfo.CreateHyperlink(link, displayName);
            var cellIndex = GetCellIndex(cellName);
            sheet.Hyperlinks.Add(cellIndex, linkInfo);
            return sheet.SetValue(cellName, displayName);
        }

        public static Worksheet SetValue(this Worksheet sheet, string cellName, string value)
        {
            if (value is null)
                return sheet;

            var index = GetCellIndex(cellName);
            var cell = sheet.Cells[index];
            cell.SetValueAsText(value);
            return sheet;
        }

        public static Worksheet SetValue(this Worksheet sheet, string cellName, int value)
        {
            var index = GetCellIndex(cellName);
            var cell = sheet.Cells[index];
            cell.SetValue(value);
            return sheet;
        }

        public static Worksheet SetValue(this Worksheet sheet, string cellName, double value)
        {
            var index = GetCellIndex(cellName);
            var cell = sheet.Cells[index];
            cell.SetValue(value);
            return sheet;
        }

        public static Worksheet SetValue(this Worksheet sheet, string cellName, DateTimeOffset value)
        {
            var index = GetCellIndex(cellName);
            var cell = sheet.Cells[index];
            cell.SetValue(value.Date);
            return sheet;
        }

        public static void UpdateCoachingForm(this Workbook workbook, UnderwritingAnalysis analysis)
        {
            var sheet = workbook.GetWorksheet(CoachingForm);

            sheet.SetValue("A4", analysis.DealAnalysis.Summary)
                .SetValue("A6", analysis.DealAnalysis.ValuePlays)
                .SetValue("C7", analysis.Vintage)
                .SetValue("A9", analysis.DealAnalysis.ConstructionType)
                .SetValue("G10", analysis.DealAnalysis.UtilityNotes)
                .SetValue("D11", analysis.DeferredMaintenance)
                .SetValue("D12", analysis.PropertyClass.Humanize(LetterCasing.Title))
                .SetValue("D13", analysis.NeighborhoodClass.Humanize(LetterCasing.Title))
                .SetValue("D24", analysis.PhysicalVacancy)
                .SetValue("D25", analysis.CostPerUnit)
                .SetValue("D26", analysis.CapRate)
                .SetValue("D27", analysis.CashOnCash)
                .SetValue("D28", analysis.AskingPrice)
                .SetValue("D29", analysis.OfferPrice)
                .SetValue("D30", analysis.StrikePrice)
                .SetValue("D32", analysis.MarketVacancy)
                .SetValue("D33", analysis.DealAnalysis.MarketPricePerUnit)
                .SetValue("D34", analysis.DealAnalysis.MarketCapRate)
                .SetValue("A36", analysis.DealAnalysis.CompetitionNotes)
                .SetValue("A40", analysis.DealAnalysis.HowUnderwritingWasDetermined);
        }

        public static void AddNotes(this Workbook workbook, UnderwritingAnalysis analysis)
        {
            var sheet = workbook.GetWorksheet(UnderwritingNotes);

            var row = 2;
            foreach (var note in analysis.Notes)
            {
                if (string.IsNullOrEmpty(note.Note?.ConvertToPlainText()))
                    continue;

                sheet.Cells[new CellIndex(2, 0)].SetValue(note.Underwriter);
                sheet.Cells[new CellIndex(2, 1)].SetValue(note.Timestamp.Date);
                sheet.Cells[new CellIndex(2, 2)].SetValue(note.Note.ConvertToPlainText());
                row++;
            }
        }

        private static CellIndex GetCellIndex(string cell)
        {
            var column = Regex.Replace(cell, @"\d+", string.Empty).ToUpper();
            var row = Regex.Replace(cell, @"[a-zA-Z]+", string.Empty);
            var columnIndex = column.Sum(x => _alphabet.IndexOf(x)) + (column.Length - 1);
            var rowIndex = int.Parse(row) - 1;
            return new CellIndex(rowIndex, columnIndex);
        }

        private static readonly List<char> _alphabet = new()
        {
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',
            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z'
        };
    }
}
