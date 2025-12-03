using System;

namespace Viasoft.Licensing.LicenseServer.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CustomServiceNameUpdaterAttribute: Attribute
    {
        public string CustomServiceNameUpdater { get; }

        public CustomServiceNameUpdaterAttribute(string customServiceNameUpdater)
        {
            CustomServiceNameUpdater = customServiceNameUpdater;
        }
    }
}