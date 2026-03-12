using ERP.BusinessModels.BaseVM;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetGeneralUserResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }
    }
}
