using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ERP.Entities.ComplexTypes
{
    
    public class spGetTotalUserRoles
    {
        [Key]
        public long Id { get; set; }
        public Guid? RoleId { get; set; }
        public string RoleName { get; set; }
        public int TotalUsers { get; set; }
        public int SortOrder { get; set; }
    }
}
