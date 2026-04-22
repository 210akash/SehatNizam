using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetPayroll
    {
        public long Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public GetStatus Status { get; set; }
        public bool IsActive { get; set; }
        public List<GetPayrollDetail> PayrollDetails { get; set; }
    }
}
