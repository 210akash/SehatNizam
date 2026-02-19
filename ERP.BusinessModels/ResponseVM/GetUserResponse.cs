using ERP.BusinessModels.BaseVM;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetUserResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? RoleId { get; set; }
        public string RoleName { get; set; }
        public string Title { get; set; }
        public long CompanyId { get; set; }
        public string ProfileBlobUrl { get; set; }
        public bool IsActive { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public DateTime? InvitationDate { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool? IsCancel { get; set; }

    }
}
