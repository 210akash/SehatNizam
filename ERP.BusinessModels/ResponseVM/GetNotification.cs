using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetNotification
    {
        public long Id { get; set; }
        public long DepartmentId { get; set; }
        public GetDepartment Department { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsExpired { get; set; }
        public bool IsActive { get; set; }
        public GetUser CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
