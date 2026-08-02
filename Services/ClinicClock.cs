namespace Family_and_Spa_Wellness.Services;

// Fargo, ND runs on Central Time (UTC-5 CDT / UTC-6 CST). The app is moving
// off localhost to a host (Render/Vercel) that typically runs its server
// clock in UTC, so appointment logic (24h cancellation window, upcoming vs.
// past, "today" for the calendar) must be computed against this zone
// explicitly rather than the ambient server clock.
public static class ClinicClock
{
    private static readonly TimeZoneInfo Zone = ResolveTimeZone();

    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);

    public static DateTime Today => Now.Date;

    private static TimeZoneInfo ResolveTimeZone()
    {
        try
        {
            // IANA id - resolves on Linux hosts and on .NET 6+ Windows.
            return TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
        }
        catch (TimeZoneNotFoundException)
        {
            // Windows-only fallback id, in case ICU/IANA mapping isn't available.
            return TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        }
    }
}
