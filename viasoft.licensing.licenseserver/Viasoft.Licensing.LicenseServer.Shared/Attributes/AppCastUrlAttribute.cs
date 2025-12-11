using System;

namespace Viasoft.Licensing.LicenseServer.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AppCastUrlAttribute: Attribute
    {
        public string AppCastUrl { get; }

        public AppCastUrlAttribute(string appCastUrl)
        {
            AppCastUrl = appCastUrl;
        }
    }
}