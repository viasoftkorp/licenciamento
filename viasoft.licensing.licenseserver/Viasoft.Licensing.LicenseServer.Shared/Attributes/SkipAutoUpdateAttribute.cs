using System;

namespace Viasoft.Licensing.LicenseServer.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class SkipAutoUpdateAttribute: Attribute
    {
        public string SkipAutoUpdate { get; }

        public bool ParsedSkipAutoUpdate => bool.TryParse(SkipAutoUpdate, out var parsed) && parsed;

        public SkipAutoUpdateAttribute(string skipAutoUpdate)
        {
            SkipAutoUpdate = skipAutoUpdate;
        }
    }
}