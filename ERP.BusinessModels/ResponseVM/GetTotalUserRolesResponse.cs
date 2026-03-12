using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetTotalUserRolesResponse
    {
        public long Id { get; set; }
        public Guid? RoleId { get; set; }
        public string RoleName { get; set; }
        public int TotalUsers { get; set; }
        public int SortOrder { get; set; }
    }
}
