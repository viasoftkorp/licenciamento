using System;

namespace Viasoft.Licensing.LicenseServer.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AutoUpdatePublicKeyAttribute: Attribute
    {
        public string AutoUpdatePublicKey { get; }

        public AutoUpdatePublicKeyAttribute(string autoUpdatePublicKey)
        {
            AutoUpdatePublicKey = autoUpdatePublicKey;
        }
    }
}