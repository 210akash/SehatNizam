using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetSection
    {
        public bool IsActive { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? RowId { get; set; }
        public virtual GetRow Row { get; set; }

        public long? CompanyId { get; set; }
        public virtual GetCompany Company { get; set; }
    }
}
