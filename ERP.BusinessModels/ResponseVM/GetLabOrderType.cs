using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetLabOrderType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long ServiceId { get; set; }
        public GetService Service { get; set; }
        public List<GetLabTestVariable> Variables { get; set; }
    }
}
