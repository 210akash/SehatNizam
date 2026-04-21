using System;
using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeSalary
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public long SalaryHeadId { get; set; }
        public string SalaryHeadName { get; set; }
        public SalaryHeadType SalaryHeadType { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public DateTime EffectiveFrom { get; set; }
    }
}
