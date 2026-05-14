using ERP.Entities.Models;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetLabTestVariableOption
    {
        public long Id { get; set; }
        public long LabTestVariableId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public LabTestVariable LabTestVariable { get; set; }
    }
}
