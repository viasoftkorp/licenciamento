using Viasoft.Licensing.LicensingManagement.Domain.Account.Enum;

namespace Viasoft.Licensing.LicensingManagement.Domain.Account.DTO
{
	public class CrmAccountOutput
	{
		public string Name { get; set; }

		public string Nickname { get; set; }

		public string Phone { get; set; }

		public string WebSite { get; set; }

		public string Description { get; set; }

		public string Email { get; set; }

		public string BillingEmail { get; set; }

		public string ClientCode { get; set; }

		public string TradingName { get; set; }

		public string CompanyName { get; set; }

		public string CnpjCpf { get; set; }

		public Address MainAddress { get; set; }

		public CrmAccountStatus Status { get; set; }
		
		public string NormalizedCnpjCpf => CnpjCpf.Replace(".", "")
			.Replace("/", "")
			.Replace("-","");
		
		public string NormalizedZipCode => MainAddress.ZipCode?.Replace("-", "")
			.Replace(".", "");
	}
}