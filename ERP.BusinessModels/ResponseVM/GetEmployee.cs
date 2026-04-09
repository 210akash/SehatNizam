using System;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployee
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HrCode { get; set; }
        public string WorkLocation { get; set; }
        public string PhoneNumber { get; set; }
        public string Supervisor { get; set; }
        public string Designation { get; set; }
        public long EmployeeShiftId { get; set; }
    }
}
