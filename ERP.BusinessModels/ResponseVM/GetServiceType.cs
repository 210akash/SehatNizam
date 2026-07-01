using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetServiceType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public GetCreatedBy CreatedBy { get; set; }
        public virtual List<GetServiceAccount> ServiceAccounts { get; set; }
    }
}
