using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class ProspectPropertyResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public UnderwritingStatus Status { get; set; }

        [JsonIgnore]
        public string FormattedLocation => Location();

        public int Units { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double CapRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double CoC { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double DebtCoverage { get; set; }

        public string Underwriter { get; set; }

        public string UnderwriterEmail { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
        public DateTimeOffset Created { get; set; }

        private string Location()
        {
            if (string.IsNullOrEmpty(City) && string.IsNullOrEmpty(State))
                return string.Empty;

            else if (string.IsNullOrEmpty(City))
                return State;

            else if (string.IsNullOrEmpty(State))
                return City;

            return $"{City}, {State}";
        }
    }
}
