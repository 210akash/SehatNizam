using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetCreatedBy
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public  long DepartmentId { get; set; }
        public  GetDepartment Department { get; set; }
    }
}
