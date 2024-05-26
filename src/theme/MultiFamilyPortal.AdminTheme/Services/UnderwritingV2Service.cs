using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace MultiFamilyPortal.AdminTheme.Services
{
    public static class UnderwritingV2Service
    {
        private const string Aquisition = "Acqusition-NF";
        private const string AquisitionFirstYear = "Year 1 Projection - NF";
        private const string Assumption = "Assumption";
        private const string AssumptionFirstYear = "Year 1 Project - Assume";

        public static byte[] GenerateUnderwritingSpreadsheet(UnderwritingAnalysis analysis, IEnumerable<UnderwritingAnalysisFile> files)
        {
            var formatProvider = new XlsxFormatProvider();
            var workbook = formatProvider.LoadWorkbook("underwriting-v2.xlsx");
            var financials = workbook.GetWorksheet(analysis.LoanType != UnderwritingLoanType.Assumption ? Aquisition : Assumption);
            var projections = workbook.GetWorksheet(analysis.LoanType != UnderwritingLoanType.Assumption ? AquisitionFirstYear : AssumptionFirstYear);
            UpdateAcquisition(financials, analysis);
            UpdateFirstYear(projections, analysis);

            if (analysis.LoanType == UnderwritingLoanType.Assumption)
            {
                var aquisitions = workbook.GetWorksheet(Aquisition);
                workbook.Sheets.Remove(aquisitions);
                var aquisitionProjection = workbook.GetWorksheet(AquisitionFirstYear);
                workbook.Sheets.Remove(aquisitionProjection);
            }
            else
            {
                var assumption = workbook.GetWorksheet(Assumption);
                workbook.Sheets.Remove(assumption);
                var assumptionProjections = workbook.GetWorksheet(AssumptionFirstYear);
                workbook.Sheets.Remove(assumptionProjections);
            }

            workbook.AddNotes(analysis);
            workbook.UpdateCoachingForm(analysis);
            workbook.AddFiles(files);

            workbook.ActiveSheet = financials;
            return formatProvider.ExportAsByteArray(workbook);
        }

        private static void UpdateFirstYear(Worksheet sheet, UnderwritingAnalysis analysis)
        {
            var factor = 1.03;
            sheet.SetValue("F13", analysis.Ours.Sum(UnderwritingCategory.GrossScheduledRent) * factor)
                .SetValue("F14", analysis.PhysicalVacancy)
                .SetValue("F15", analysis.Ours.Sum(UnderwritingCategory.ConsessionsNonPayment) * factor)
                .SetValue("F17", analysis.Ours.Sum(UnderwritingCategory.UtilityReimbursement) * factor)
                .SetValue("F18", analysis.Ours.Sum(UnderwritingCategory.OtherIncome, UnderwritingCategory.OtherIncomeBad) * factor);

            sheet.SetValue("F22", analysis.Ours.Sum(UnderwritingCategory.Taxes) * factor)
                .SetValue("F23", analysis.Ours.Sum(UnderwritingCategory.Insurance) * factor)
                .SetValue("F24", analysis.Ours.Sum(UnderwritingCategory.RepairsMaintenance) * factor)
                .SetValue("F25", analysis.Ours.Sum(UnderwritingCategory.GeneralAdmin) * factor)
                .SetValue("F27", analysis.Ours.Sum(UnderwritingCategory.Marketing) * factor)
                .SetValue("F28", analysis.Ours.Sum(UnderwritingCategory.Utility) * factor)
                .SetValue("F29", analysis.Ours.Sum(UnderwritingCategory.ContractServices) * factor)
                .SetValue("F30", analysis.Ours.Sum(UnderwritingCategory.Payroll) * factor);
        }

        private static void UpdateAcquisition(Worksheet sheet, UnderwritingAnalysis analysis)
        {
            sheet.SetValue("F49", analysis.Timestamp)
                .SetValue("C50", analysis.Name)
                .SetValue("D51", analysis.Units)
                .SetValue("D52", analysis.PurchasePrice)
                .SetValue("D54", analysis.RentableSqFt)
                .SetValue("G53", analysis.OfferPrice)
                .SetValue("G54", analysis.StrikePrice)
                .SetValue("G55", analysis.AskingPrice)
                .SetValue("M51", analysis.GrossPotentialRent);

            var vacancy = 0.05;
            if (analysis.PhysicalVacancy >= 0.05)
                vacancy = analysis.PhysicalVacancy;
            else if (analysis.MarketVacancy > 0.05)
                vacancy = analysis.MarketVacancy;

            sheet.SetValue("D58", analysis.Ours.Sum(UnderwritingCategory.GrossScheduledRent))
                .SetValue("C59", vacancy)
                .SetValue("D60", analysis.Ours.Sum(UnderwritingCategory.ConsessionsNonPayment))
                .SetValue("D62", analysis.Ours.Sum(UnderwritingCategory.UtilityReimbursement))
                .SetValue("D63", analysis.Ours.Sum(UnderwritingCategory.OtherIncome, UnderwritingCategory.OtherIncomeBad));

            sheet.SetValue("F58", analysis.Sellers.Sum(UnderwritingCategory.GrossScheduledRent))
                .SetValue("F59", analysis.Sellers.Sum(UnderwritingCategory.PhysicalVacancy))
                .SetValue("F60", analysis.Sellers.Sum(UnderwritingCategory.ConsessionsNonPayment))
                .SetValue("F62", analysis.Sellers.Sum(UnderwritingCategory.UtilityReimbursement))
                .SetValue("F63", analysis.Sellers.Sum(UnderwritingCategory.OtherIncome, UnderwritingCategory.OtherIncomeBad, UnderwritingCategory.OtherIncomeOneTime));

            sheet.SetValue("D67", analysis.Ours.Sum(UnderwritingCategory.Taxes))
                .SetValue("D68", analysis.Ours.Sum(UnderwritingCategory.Insurance))
                .SetValue("D69", analysis.Ours.Sum(UnderwritingCategory.RepairsMaintenance))
                .SetValue("D70", analysis.Ours.Sum(UnderwritingCategory.GeneralAdmin))
                .SetValue("C71", analysis.Management)
                .SetValue("D72", analysis.Ours.Sum(UnderwritingCategory.Marketing))
                .SetValue("D73", analysis.Ours.Sum(UnderwritingCategory.Utility))
                .SetValue("D74", analysis.Ours.Sum(UnderwritingCategory.ContractServices))
                .SetValue("D75", analysis.Ours.Sum(UnderwritingCategory.Payroll));

            sheet.SetValue("F67", analysis.Sellers.Sum(UnderwritingCategory.Taxes))
                .SetValue("F68", analysis.Sellers.Sum(UnderwritingCategory.Insurance))
                .SetValue("F69", analysis.Sellers.Sum(UnderwritingCategory.RepairsMaintenance))
                .SetValue("F70", analysis.Sellers.Sum(UnderwritingCategory.GeneralAdmin))
                .SetValue("F71", analysis.Sellers.Sum(UnderwritingCategory.Management))
                .SetValue("F72", analysis.Sellers.Sum(UnderwritingCategory.Marketing))
                .SetValue("F73", analysis.Sellers.Sum(UnderwritingCategory.Utility))
                .SetValue("F74", analysis.Sellers.Sum(UnderwritingCategory.ContractServices))
                .SetValue("F75", analysis.Sellers.Sum(UnderwritingCategory.Payroll));

            sheet.SetValue("E79", analysis.CapXTotal / analysis.Units)
                .SetValue("E87", analysis.OurEquityOfCF);

            sheet.SetValue("L67", analysis.ClosingCostPercent)
                .SetValue("L70", analysis.AquisitionFeePercent);
                //.SetValue("M26", analysis.ClosingCostOther);

            var mortgage = analysis.Mortgages.First();
            if (analysis.LoanType != UnderwritingLoanType.Assumption)
            {
                sheet.SetValue("M68", mortgage.Points);
                sheet.SetValue("M59", 1 - (mortgage.LoanAmount / analysis.PurchasePrice));
                sheet.SetValue("M62", mortgage.InterestRate);
                sheet.SetValue("M63", mortgage.TermInYears);
            }
            else
            {
                sheet.SetValue("M59", mortgage.LoanAmount)
                    .SetValue("M62", mortgage.AnnualDebtService);
            }

            var secondMortgage = analysis.Mortgages.Skip(1).FirstOrDefault() ?? new UnderwritingAnalysisMortgage();
            sheet.SetValue("M77", secondMortgage.LoanAmount)
                .SetValue("M78", secondMortgage.InterestRate)
                .SetValue("M79", secondMortgage.TermInYears);

            var taxes = analysis.Ours.Sum(UnderwritingCategory.Taxes);
            var insurance = analysis.Ours.Sum(UnderwritingCategory.Insurance);

            var operatingExpenses = analysis.Ours
                .Where(x => x.Category.GetLineItemType() == UnderwritingType.Expense && x.Category != UnderwritingCategory.Insurance)
                .Sum(x => x.AnnualizedTotal);

            sheet.SetValue("M83", operatingExpenses / 6)
                .SetValue("M84", analysis.CapXTotal)
                .SetValue("M85", analysis.DeferredMaintenance)
                .SetValue("M86", insurance)
                .SetValue("M87", taxes)
                .SetValue("M88", analysis.SECAttorney);
        }
    }
}
