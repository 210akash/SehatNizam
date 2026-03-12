using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ERP.BusinessModels.BaseVM
{
    
    public partial class UserInvitationsModel
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string UserEmail { get; set; }
        public Guid RoleId { get; set; }
    }
}
