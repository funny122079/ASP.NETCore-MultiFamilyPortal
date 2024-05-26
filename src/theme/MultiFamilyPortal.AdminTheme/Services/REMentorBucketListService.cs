using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;

namespace MultiFamilyPortal.AdminTheme.Services
{
    public class REMentorBucketListService
    {
        public static byte[] GenerateBucketlist(UnderwritingAnalysis property)
        {
            var formatProvider = new XlsxFormatProvider();
            var workbook = formatProvider.LoadWorkbook("bucketlist.xlsx");

            var bucketlist = workbook.GetWorksheet("Bucketlist");
            bucketlist.SetValue("C3", property.Name)
                .SetValue("C4", DateTime.Now.Date)
                .SetValue("B8", property.GrossPotentialRentNotes)
                .SetValue("B11", property.LossToLeaseNotes)
                .SetValue("B14", property.GrossScheduledRentNotes)
                .SetValue("B17", property.PhysicalVacancyNotes)
                .SetValue("B23", property.ConcessionsNonPaymentNotes)
                .SetValue("B31", property.UtilityReimbursementNotes)
                .SetValue("B36", property.OtherIncomeNotes)
                .SetValue("B46", property.TaxesNotes)
                .SetValue("C46", property.MarketingNotes)
                .SetValue("B50", property.InsuranceNotes)
                .SetValue("C52", property.UtilityNotes)
                .SetValue("B55", property.RepairsMaintenanceNotes)
                .SetValue("C58", property.ContractServicesNotes)
                .SetValue("C64", property.PayrollNotes)
                .SetValue("B67", property.GeneralAdminNotes)
                .SetValue("C73", property.ManagementNotes)
                .SetValue("B77", property.LendingNotes)
                .SetValue("C81", property.OperatingExpenses / 6)
                .SetValue("C82", property.PropertyInsurance)
                .SetValue("C83", property.DeferredMaintenance)
                .SetValue("C84", property.CapXTotal)
                .SetValue("C85", 0)
                .SetValue("C86", property.SECAttorney)
                .SetValue("C87", property.ClosingCostMiscellaneous);

            return formatProvider.ExportAsByteArray(workbook);
        }
    }
}
