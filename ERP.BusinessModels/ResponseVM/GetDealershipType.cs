using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetDealershipType
    {
        public bool IsActive { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
