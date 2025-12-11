namespace Viasoft.Licensing.LicensingManagement.Host.Extensions;

public static class EnsureSlash
{
    public static string EnsureNotTrailingSlash(this string input) => input.EndsWith("/") ? input.TrimEnd('/') : input;
}