using System.Collections;

namespace Viasoft.Licensing.LicenseServer.Domain.Extensions
{
    public static class ComparisionExtension
    {
        public static bool In<T>(this T val, params T[] values) => val != null && ((IList) values).Contains(val);
    }
}