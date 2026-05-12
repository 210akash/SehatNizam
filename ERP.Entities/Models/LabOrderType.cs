using System.Collections.Generic;

namespace ERP.Entities.Models
{
    public class LabOrderType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long ServiceId { get; set; }  
        public Service Service { get; set; }
        public ICollection<LabTestVariable> Variables { get; set; } = new List<LabTestVariable>();
    }
}
