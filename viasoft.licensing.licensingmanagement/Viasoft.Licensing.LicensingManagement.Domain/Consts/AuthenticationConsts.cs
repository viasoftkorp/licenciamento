namespace Viasoft.Licensing.LicensingManagement.Domain.Consts
{
    public static class AuthenticationConsts
    {
        public static string ServiceName => "Viasoft.Authentication";
        private static string BasePath => "oauth/";
        public static class Users
        {
            private static string Path => $"{BasePath}users";
            public static string GetAll => $"{Path}";
            public static string GetByEmail(string email) => $"{Path}/{email}";
        }
    }
}