using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetPayrollDetail
    {
        public long Id { get; set; }
        public long PayrollId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public long SalaryHeadId { get; set; }
        public string SalaryHeadName { get; set; }
        public SalaryHeadType SalaryHeadType { get; set; }
        public string SalaryHeadTypeName => SalaryHeadType.ToString();
        public decimal Amount { get; set; }
    }
}
