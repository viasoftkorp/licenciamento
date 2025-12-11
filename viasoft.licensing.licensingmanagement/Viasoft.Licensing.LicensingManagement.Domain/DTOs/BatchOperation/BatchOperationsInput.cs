using Viasoft.Data.Attributes;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation
{
    public class BatchOperationsInput
    {
        public InsertBatchOperationsInput IdsToInsert { get; set; }
        
        public InsertBatchOperationsInput IdsWhereTheyWillBeInserted { get; set; }
        
        [StrictRequired(AllowNegativeNumeric = false, AllowZeroNumeric = true)]
        public int NumberOfLicenses { get; set; }
    }
}
