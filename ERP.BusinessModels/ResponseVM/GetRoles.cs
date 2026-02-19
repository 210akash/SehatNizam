using ERP.BusinessModels.BaseVM;
using System;
using System.Collections.Generic;
using System.Text;
using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetRoles
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
