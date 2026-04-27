using System;
using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetEmployeeSalary
    {
        public long Id { get; set; }
        public Guid EmployeeId { get; set; }
        public long SalaryHeadId { get; set; }
        public SalaryHeadType SalaryHead { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public DateTime EffectiveFrom { get; set; }
    }
}
