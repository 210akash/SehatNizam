using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ERP.Entities.ComplexTypes
{
    public class spRegisterWithEmail
    {
        public string ErrorMessage { get; set; }
        public Guid? RoleId { get; set; }
        public int? CompanyId { get; set; }
        [Key]
        public long Id { get; set; }
    }
}
