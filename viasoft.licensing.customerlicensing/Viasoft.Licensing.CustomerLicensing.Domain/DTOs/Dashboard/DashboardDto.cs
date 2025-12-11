using System;

namespace Viasoft.Licensing.CustomerLicensing.Domain.DTOs.Dashboard
{
    public class DashboardDto
    {
        public Guid ConsumerId {get; set;}
        public byte[] SerializedDashboard {get; set;}
    }
}