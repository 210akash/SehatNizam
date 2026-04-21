using System.Collections.Generic;
using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetPayroll
    {
        public long Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public PayrollStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public bool IsActive { get; set; }
        public List<GetPayrollDetail> PayrollDetails { get; set; }
    }
}
