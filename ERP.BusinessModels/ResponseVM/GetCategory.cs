using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetCategory
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
        public GetUser CreatedBy { get; set; }
        public List<long> StoreIds { get; set; }
        public List<GetCategoryStore> CategoryStores { get; set; }
    }
}
