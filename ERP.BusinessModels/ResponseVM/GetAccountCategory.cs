using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetAccountCategory
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public long AccountHeadId { get; set; }
        public GetAccountHead AccountHead { get; set; }

        public long CompanyId { get; set; }
        public GetCompany Company { get; set; }
        public GetUser CreatedBy { get; set; }
        public List<long> StoreIds { get; set; }
    }
}
