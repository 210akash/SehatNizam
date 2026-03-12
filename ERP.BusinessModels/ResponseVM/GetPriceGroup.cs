using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetPriceGroup
    {
        public bool IsActive { get; set; }
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
