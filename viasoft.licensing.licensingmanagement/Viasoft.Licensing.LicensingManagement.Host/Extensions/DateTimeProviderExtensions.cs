using System;
using Viasoft.Core.DateTimeProvider;

namespace Viasoft.Licensing.LicensingManagement.Host.Extensions;

public static class DateTimeProviderExtensions
{
    public static long GetUnixTimeTicks(this IDateTimeProvider dateTimeProvider)
    {
        var dateTimeOffset = (DateTimeOffset)DateTime.SpecifyKind(dateTimeProvider.UtcNow(), DateTimeKind.Utc);
        return dateTimeOffset.ToUnixTimeSeconds();
    }
}