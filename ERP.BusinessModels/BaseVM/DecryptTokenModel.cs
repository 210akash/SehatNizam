using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.BusinessModels.BaseVM
{
   public class DecryptTokenModel
    {
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public int? IndustryId { get; set; }
        public string Title { get; set; }
        public Guid RoleId { get; set; }
    }
}
