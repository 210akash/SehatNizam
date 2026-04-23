using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetSalaryHead
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsTaxable { get; set; }
        public SalaryHeadType Type { get; set; }
        public string TypeName => Type.ToString();
        public bool IsActive { get; set; }
    }
}
