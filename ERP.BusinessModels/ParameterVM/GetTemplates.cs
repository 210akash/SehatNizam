using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BusinessModels.ParameterVM
{
    public class GetTemplates
    {
        public long Id { get; set; }
        public Guid CreatedById { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public DateTime? CreatedDate { get; set; }
        public Guid ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string Name { get; set; }
        //public string Description { get; set; }
        public string Content { get; set; }
        //public int Society_id { get; set; }
        //public int Code { get; set; }
        //public int Type { get; set; }
    }
}
