using System;

namespace Viasoft.Licensing.LicenseServer.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AuthorityAttribute: Attribute
    {
        public string Authority { get; }

        public AuthorityAttribute(string authority)
        {
            Authority = authority;
        }
    }
}