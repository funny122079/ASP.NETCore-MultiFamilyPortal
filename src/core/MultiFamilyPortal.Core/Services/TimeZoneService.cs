using Microsoft.JSInterop;
using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.Services
{
    internal class TimeZoneService : ITimeZoneService
    {
        private IJSRuntime _jsRuntime { get; }

        private TimeSpan? _userOffset;

        private DateTime? _dateTime = null;

        public TimeZoneService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async ValueTask<DateTimeOffset> GetLocalDateTime(DateTimeOffset dateTime)
        {
            if (_userOffset is null)
            {
                var offsetInMinutes = await _jsRuntime.InvokeAsync<int>("MFPortal.LocalTime");
                _userOffset = TimeSpan.FromMinutes(offsetInMinutes);
            }

            return dateTime.ToOffset(_userOffset.Value);
        }

        public async ValueTask<DateTime> GetLocalFullDate()
        {
            if (_dateTime is null)
                 _dateTime = ConvertJsDate(await _jsRuntime.InvokeAsync<string>("MFPortal.LocalDate"));
            return (DateTime)_dateTime;
        }

        public DateTime GetLocalTimeByTimeZone(string timezone) => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById(timezone));

        public List<TimezoneData> Timezones => _allTimeZones;

        private readonly List<TimezoneData> _allTimeZones = new()
        {
            new TimezoneData { Name = "Nuie Time", Intials = "NUT", UTC = "UTC-11", },
            new TimezoneData { Name = "Aleutian Standard Time", Intials = "HST", UTC = "UTC-10", },
            new TimezoneData { Name = "Hawaiian Standard Time", Intials = "HST", UTC = "UTC-10", },
            new TimezoneData { Name = "Marquesas Standard Time", Intials = "MIT", UTC = "UTC-09:30", },
            new TimezoneData { Name = "Alaskan Standard Time", Intials = "AKST", UTC = "UTC-09", },
            new TimezoneData { Name = "Alaskan Daylight Time", Intials = "AKDT", UTC = "UTC-08", },
            new TimezoneData { Name = "Pacific Daylight Time", Intials = "PDT", UTC = "UTC-07", },
            new TimezoneData { Name = "Pacific Standard Time", Intials = "PST", UTC = "UTC-08", },
            new TimezoneData { Name = "US Mountain Standard Time", Intials = "MDT", UTC = "UTC-07", },
            new TimezoneData { Name = "Mountain Standard Time", Intials = "MST", UTC = "UTC-07", },
            new TimezoneData { Name = "Central America Standard Time", Intials = "CDT", UTC = "UTC-6", },
            new TimezoneData { Name = "Central Standard Time", Intials = "CST", UTC = "UTC-6", },
            new TimezoneData { Name = "Easter Island Standard Time", Intials = "EAST", UTC = "UTC-06", },
            new TimezoneData { Name = "Central Standard Time (Mexico)", Intials = "CDT", UTC = "UTC-06", },
            new TimezoneData { Name = "Canada Central Standard Time", Intials = "DST", UTC = "UTC-05", },
            new TimezoneData { Name = "SA Pacific Standard Time", Intials = "PSA", UTC = "UTC-04", },
            new TimezoneData { Name = "Eastern Standard Time (Mexico)", Intials = "EST", UTC = "UTC-05", },
            new TimezoneData { Name = "Eastern Standard Time", Intials = "EST", UTC = "UTC-05", },
            new TimezoneData { Name = "Haiti Standard Time", Intials = "EST", UTC = "UTC-05", },
            new TimezoneData { Name = "Cuba Standard Time", Intials = "CST", UTC = "UTC-05", },
            new TimezoneData { Name = "US Eastern Standard Time", Intials = "EST", UTC = "UTC-05", },
            new TimezoneData { Name = "Paraguay Standard Time", Intials = "PYT", UTC = "UTC-04", },
            new TimezoneData { Name = "Atlantic Standard Time", Intials = "AST", UTC = "UTC-04", },
            new TimezoneData { Name = "Venezuela Standard Time", Intials = "VET", UTC = "UTC-04", },
            new TimezoneData { Name = "Central Brazilian Standard Time", Intials = "CBST", UTC = "UTC-04", },
            new TimezoneData { Name = "SA Western Standard Time", Intials = "SAWST", UTC = "UTC-04", },
            new TimezoneData { Name = "Pacific SA Standard Time", Intials = "PSA", UTC = "UTC-04", },
            new TimezoneData { Name = "Turks And Caicos Standard Time", Intials = "EST", UTC = "UTC-05", },
            new TimezoneData { Name = "Newfoundland Standard Time", Intials = "NLT", UTC = "UTC-03:30", },
            new TimezoneData { Name = "Tocantins Standard Time", Intials = "BRT", UTC = "UTC-03", },
            new TimezoneData { Name = "E. South America Standard Time", Intials = "SAEST", UTC = "UTC-03", },
            new TimezoneData { Name = "SA Eastern Standard Time", Intials = "SAEST", UTC = "UTC-03", },
            new TimezoneData { Name = "Greenland Standard Time", Intials = "AST", UTC = "UTC-4", },
            new TimezoneData { Name = "Montevideo Standard Time", Intials = "MVD", UTC = "UTC-03", },
            new TimezoneData { Name = "Saint Pierre Standard Time", Intials = "PMST", UTC = "UTC-3", },
            new TimezoneData { Name = "Bahia Standard Time", Intials = "BRT", UTC = "UTC-03", },
            new TimezoneData { Name = "Fernando de Noronha Time", Intials = "FNT", UTC = "UTC-02", },
            new TimezoneData { Name = "Mid-Atlantic Standard Time", Intials = "MAST", UTC = "UTC-02", },
            new TimezoneData { Name = "Azores Standard Time", Intials = "AZOST", UTC = "UTC-01", },
            new TimezoneData { Name = "Cape Verde Standard Time", Intials = "CVT", UTC = "UTC-1", },
            new TimezoneData { Name = "GMT Standard Time", Intials = "GMT", UTC = "UTC", },
            new TimezoneData { Name = "Morocco Standard Time", Intials = "MRC", UTC = "UTC", },
            new TimezoneData { Name = "Greenwich Standard Time", Intials = "GST", UTC = "UTC", },
            new TimezoneData { Name = "W. Europe Standard Time", Intials = "WET", UTC = "UTC+01", },
            new TimezoneData { Name = "Central Europe Standard Time", Intials = "CET", UTC = "UTC+01", },
            new TimezoneData { Name = "Romance Standard Time", Intials = "ROM", UTC = "UTC+01", },
            new TimezoneData { Name = "Central European Standard Time", Intials = "CEST", UTC = "UTC+02", },
            new TimezoneData { Name = "W. Central Africa Standard Time", Intials = "WCAST", UTC = "UTC+01", },
            new TimezoneData { Name = "Namibia Standard Time", Intials = "NMT", UTC = "UTC+02", },
            new TimezoneData { Name = "Jordan Standard Time", Intials = "EET", UTC = "UTC+02", },
            new TimezoneData { Name = "GTB Standard Time", Intials = "GTB", UTC = "UTC+02", },
            new TimezoneData { Name = "Middle East Standard Time", Intials = "MEST", UTC = "UTC+02", },
            new TimezoneData { Name = "Egypt Standard Time", Intials = "EGY", UTC = "UTC+02", },
            new TimezoneData { Name = "E. Europe Standard Time", Intials = "EET", UTC = "UTC+02", },
            new TimezoneData { Name = "Syria Standard Time", Intials = "EET", UTC = "UTC+02", },
            new TimezoneData { Name = "West Bank Standard Time", Intials = "EET", UTC = "UTC+02", },
            new TimezoneData { Name = "South Africa Standard Time", Intials = "SAST", UTC = "UTC+02", },
            new TimezoneData { Name = "FLE Standard Time", Intials = "FLE", UTC = "UTC+02", },
            new TimezoneData { Name = "Israel Standard Time", Intials = "IST", UTC = "UTC+02", },
            new TimezoneData { Name = "Kaliningrad Standard Time", Intials = "EET", UTC = "UTC+02", },
            new TimezoneData { Name = "Libya Standard Time", Intials = "EET", UTC = "UTC+02", },
            new TimezoneData { Name = "Arabic Standard Time", Intials = "ARABIC", UTC = "UTC+03", },
            new TimezoneData { Name = "Turkey Standard Time", Intials = "TRT", UTC = "UTC+03", },
            new TimezoneData { Name = "Arab Standard Time", Intials = "ARAB", UTC = "UTC+03", },
            new TimezoneData { Name = "Belarus Standard Time", Intials = "MKS", UTC = "UTC+03", },
            new TimezoneData { Name = "E. Africa Standard Time", Intials = "EAT", UTC = "UTC+03", },
            new TimezoneData { Name = "Iran Standard Time", Intials = "IRST", UTC = "UTC+03:30", },
            new TimezoneData { Name = "Arabian Standard Time", Intials = "ARABIA", UTC = "UTC+04", },
            new TimezoneData { Name = "Astrakhan Standard Time", Intials = "MSK+1", UTC = "UTC+4", },
            new TimezoneData { Name = "Azerbaijan Standard Time", Intials = "AZT", UTC = "UTC+04", },
            new TimezoneData { Name = "Russia Time Zone 3", Intials = "MSK", UTC = "UTC+03", },
            new TimezoneData { Name = "Mauritius Standard Time", Intials = "MUT", UTC = "UTC+4", },
            new TimezoneData { Name = "Georgian Standard Time", Intials = "GET", UTC = "UTC+04", },
            new TimezoneData { Name = "Caucasus Standard Time", Intials = "CST", UTC = "UTC+04", },
            new TimezoneData { Name = "Afghanistan Standard Time", Intials = "AFT", UTC = "UTC+05:30", },
            new TimezoneData { Name = "West Asia Standard Time", Intials = "WAT", UTC = "UTC+05", },
            new TimezoneData { Name = "Ekaterinburg Standard Time", Intials = "YEKT", UTC = "UTC=05", },
            new TimezoneData { Name = "Pakistan Standard Time", Intials = "PKT", UTC = "UTC+05", },
            new TimezoneData { Name = "India Standard Time", Intials = "IST", UTC = "UTC+05:30", },
            new TimezoneData { Name = "Sri Lanka Standard Time", Intials = "SLST", UTC = "UTC+05:30", },
            new TimezoneData { Name = "Nepal Standard Time", Intials = "NPT", UTC = "UTC+05:45", },
            new TimezoneData { Name = "Central Asia Standard Time", Intials = "CAT", UTC = "UTC+06", },
            new TimezoneData { Name = "Bangladesh Standard Time", Intials = "BST", UTC = "UTC+06", },
            new TimezoneData { Name = "Omsk Standard Time", Intials = "OMST", UTC = "UTC+06", },
            new TimezoneData { Name = "Myanmar Standard Time", Intials = "MMT", UTC = "UTC+06:30", },
            new TimezoneData { Name = "SE Asia Standard Time", Intials = "SEA", UTC = "UTC+07", },
            new TimezoneData { Name = "Altai Standard Time", Intials = "UTC+07", UTC = "", },
            new TimezoneData { Name = "W. Mongolia Standard Time", Intials = "WITA", UTC = "UTC+08", },
            new TimezoneData { Name = "North Asia Standard Time", Intials = "NAST", UTC = "UTC+07", },
            new TimezoneData { Name = "N. Central Asia Standard Time", Intials = "NCAST", UTC = "UTC+06", },
            new TimezoneData { Name = "Tomsk Standard Time", Intials = "MSK", UTC = "UTC+7", },
            new TimezoneData { Name = "China Standard Time", Intials = "CHN", UTC = "UTC+08", },
            new TimezoneData { Name = "North Asia East Standard Time", Intials = "NAST", UTC = "UTC+07", },
            new TimezoneData { Name = "Singapore Standard Time", Intials = "SST", UTC = "UTC+08", },
            new TimezoneData { Name = "W. Australia Standard Time", Intials = "AWST", UTC = "UTC+08", },
            new TimezoneData { Name = "Taipei Standard Time", Intials = "CST", UTC = "UTC+08", },
            new TimezoneData { Name = "Ulaanbaatar Standard Time", Intials = "ULAT", UTC = "UTC+08", },
            new TimezoneData { Name = "North Korea Standard Time", Intials = "NKST", UTC = "UTC+09", },
            new TimezoneData { Name = "Aus Central W. Standard Time", Intials = "ACT", UTC = "UTC+09", },
            new TimezoneData { Name = "Transbaikal Standard Time", Intials = "TST", UTC = "UTC+09", },
            new TimezoneData { Name = "Tokyo Standard Time", Intials = "JST", UTC = "UTC+09", },
            new TimezoneData { Name = "Korea Standard Time", Intials = "KST", UTC = "UTC+09", },
            new TimezoneData { Name = "Yakutsk Standard Time", Intials = "YAKT", UTC = "UTC+09:00", },
            new TimezoneData { Name = "Cen. Australia Standard Time", Intials = "ACDT", UTC = "UTC+09:30", },
            new TimezoneData { Name = "AUS Central Standard Time", Intials = "ACST", UTC = "UTC+09:30", },
            new TimezoneData { Name = "AUS Eastern Standard Time", Intials = "AEST", UTC = "UTC+10", },
            new TimezoneData { Name = "West Pacific Standard Time", Intials = "WPT", UTC = "UTC+10", },
            new TimezoneData { Name = "Tasmania Standard Time", Intials = "AEST", UTC = "UTC+10", },
            new TimezoneData { Name = "Vladivostok Standard Time", Intials = "VVS", UTC = "UTC+10", },
            new TimezoneData { Name = "Lord Howe Standard Time", Intials = "LHST", UTC = "UTC+10:30", },
            new TimezoneData { Name = "Bougainville Standard Time", Intials = "BST", UTC = "UTC+11", },
            new TimezoneData { Name = "Russia Time Zone 10", Intials = "MAGT", UTC = "UTC+11", },
            new TimezoneData { Name = "Magadan Standard Time", Intials = "MAGT", UTC = "UTC+12", },
            new TimezoneData { Name = "Norfolk Standard Time", Intials = "NFT", UTC = "UTC+11", },
            new TimezoneData { Name = "Sakhalin Standard Time", Intials = "SAKT", UTC = "UTC+11", },
            new TimezoneData { Name = "Central Pacific Standard Time", Intials = "CPST", UTC = "UTC+11", },
            new TimezoneData { Name = "Russia Time Zone 11", Intials = "MAGT", UTC = "UTC+11", },
            new TimezoneData { Name = "New Zealand Standard Time", Intials = "NZST", UTC = "UTC+12", },
            new TimezoneData { Name = "Gilbert Island Time", Intials = "GILT", UTC = "UTC+12", },
            new TimezoneData { Name = "Fiji Standard Time", Intials = "FTST", UTC = "+12", },
            new TimezoneData { Name = "Kamchatka Standard Time", Intials = "ANAT", UTC = "UTC+12", },
            new TimezoneData { Name = "Marshall Islands Time", Intials = "MHT", UTC = "UTC+12", },
            new TimezoneData { Name = "Chatham Islands Standard Time", Intials = "CHAST", UTC = "UTC+12:45", },
            new TimezoneData { Name = "Tonga Standard Time", Intials = "TOT", UTC = "UTC+13", },
            new TimezoneData { Name = "Samoa Standard Time", Intials = "SST", UTC = "UTC-11", },
            new TimezoneData { Name = "Line Islands Standard Time", Intials = "LINT", UTC = "UTC+14", }
        };

        private static DateTime ConvertJsDate(string jsDate)
        {
            const string formatString = "ddd MMM d yyyy HH:mm:ss";

            var gmtIndex = jsDate.IndexOf(" GMT");
            switch (gmtIndex)
            {
                case > -1:
                    jsDate = jsDate.Remove(gmtIndex);
                    return DateTime.ParseExact(jsDate, formatString, null);
                default:
                    return DateTime.Parse(jsDate);
            }
        }
    }
}
