using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ERP.Entities.Models
{
    public partial class UserInvitations
    {
        [Key]
        public Guid Id { get; set; }
        public long? CompanyId { get; set; }
        public string UserEmail { get; set; }
        public Guid RoleId { get; set; }
        public bool IsPending { get; set; }
        public string EmailConfirmationCode { get; set; }
        public DateTime? EmailCodeExpiryDateTime { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? ReminderDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public bool? IsCancel { get; set; }
    }
}
