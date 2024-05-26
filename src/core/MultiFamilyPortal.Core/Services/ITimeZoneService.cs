using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.Services
{
    public interface ITimeZoneService
    {
        ValueTask<DateTimeOffset> GetLocalDateTime(DateTimeOffset dateTime);

        ValueTask<DateTime> GetLocalFullDate();

        DateTime GetLocalTimeByTimeZone(string timezone);

        List<TimezoneData> Timezones { get; }
    }
}
