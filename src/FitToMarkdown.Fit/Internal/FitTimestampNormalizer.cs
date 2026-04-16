namespace FitToMarkdown.Fit.Internal;

/// <summary>
/// Converts FIT SDK timestamps to standard .NET date/time types.
/// </summary>
internal static class FitTimestampNormalizer
{
    /// <summary>
    /// Converts a FIT SDK DateTime to a UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="fitDateTime">The FIT SDK DateTime instance, or <c>null</c>.</param>
    /// <returns>A UTC <see cref="DateTimeOffset"/>, or <c>null</c> if the input is <c>null</c>.</returns>
    public static DateTimeOffset? ToUtcDateTimeOffset(Dynastream.Fit.DateTime? fitDateTime)
    {
        if (fitDateTime is null)
        {
            return null;
        }

        // The SDK's GetDateTime() returns a System.DateTime in UTC
        var utcDateTime = fitDateTime.GetDateTime();
        return new DateTimeOffset(utcDateTime, TimeSpan.Zero);
    }

    /// <summary>
    /// Derives the local time offset from UTC and local FIT timestamps.
    /// </summary>
    /// <param name="utcTimestamp">The UTC timestamp from the FIT file.</param>
    /// <param name="localTimestamp">The local timestamp from the ActivityMesg.</param>
    /// <returns>The offset between local and UTC time, or <c>null</c> if either input is <c>null</c>.</returns>
    public static TimeSpan? DeriveLocalTimeOffset(Dynastream.Fit.DateTime? utcTimestamp, Dynastream.Fit.DateTime? localTimestamp)
    {
        if (utcTimestamp is null || localTimestamp is null)
        {
            return null;
        }

        var utcDt = utcTimestamp.GetDateTime();
        var localDt = localTimestamp.GetDateTime();

        return localDt - utcDt;
    }
}
