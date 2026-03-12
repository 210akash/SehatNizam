using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetHRDashboardData
    {
        public long TotalEmployee { get; set; }
        public long NewThisMonth { get; set; }
        public long ResignedThisMonth { get; set; }
        public long SaleEmployees { get; set; }
        public long SaleFieldWorkers { get; set; }

        public List<GetDepartmentWiseCount> GetDepartmentWiseCount { get; set; }
    }

    public class GetDepartmentWiseCount
    {
        public string Department { get; set; }
        public long Count { get; set; }
    }

    public class GetTodayAttendance
    {
        public long Present { get; set; }
        public long Absent { get; set; }
        public long LeaveAppliedManager { get; set; }
        public long LeaveAppliedHR { get; set; }
        public long OnLeave { get; set; }
    }


}
