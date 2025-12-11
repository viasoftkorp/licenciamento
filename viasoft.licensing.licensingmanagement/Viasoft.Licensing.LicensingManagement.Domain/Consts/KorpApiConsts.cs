namespace Viasoft.Licensing.LicensingManagement.Domain.Consts
{
    public class KorpApiConsts
    {
        public static string GetCompanyByCnpj(string cnpj) => $"https://api-web.korp.com.br/register/cnpj/?cnpj={cnpj}";
    }
}