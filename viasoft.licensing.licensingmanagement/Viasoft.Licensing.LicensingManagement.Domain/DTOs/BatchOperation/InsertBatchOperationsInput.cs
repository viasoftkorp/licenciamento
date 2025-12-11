using System;
using System.Collections.Generic;

namespace Viasoft.Licensing.LicensingManagement.Domain.DTOs.BatchOperation
{
    public class InsertBatchOperationsInput
    {
        public InsertBatchOperationsInput()
        {
            Ids = new List<Guid>();
            UnselectedList = new List<Guid>();
        }
        public List<Guid> Ids { get; set; }
        
        public List<Guid> UnselectedList { get; set; }

        public bool AllSelected { get; set; }
        
        public string AdvancedFilter { get; set; }
    }
}