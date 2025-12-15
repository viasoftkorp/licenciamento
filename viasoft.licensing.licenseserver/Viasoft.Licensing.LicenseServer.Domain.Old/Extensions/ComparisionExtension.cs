using System;
using System.Collections;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Extensions
{
    public static class ComparisionExtension
    {
        public static bool In<T>(this T val, params T[] values) => val != null && ((IList) values).Contains(val);

        public static bool EqualsIgnoringCase(this string value, string valueToCompare) => string.Equals(value, valueToCompare, StringComparison.CurrentCultureIgnoreCase);
    }
}