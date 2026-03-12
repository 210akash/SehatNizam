using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetRow
    {
        public bool IsActive { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public long? RackId { get; set; }
        public virtual GetRack Rack { get; set; }
        public long? CompanyId { get; set; }
        public virtual GetCompany Company { get; set; }
    }
}
