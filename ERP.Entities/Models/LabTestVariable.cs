using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Entities.Models
{
    public class LabTestVariable : BaseEntity
    {
        public long LabOrderTypeId { get; set; }
        public string Name { get; set; }   // Hb
        public string Unit { get; set; }   // g/dL
        public decimal? MaleMin { get; set; }
        public decimal? MaleMax { get; set; }
        public decimal? FemaleMin { get; set; }
        public decimal? FemaleMax { get; set; }
        public bool HasGenderRange { get; set; }
        public int DisplayOrder { get; set; }
        public ResultType ResultType { get; set; }
        public LabOrderType LabOrderType { get; set; }
        public List<LabTestVariableOption> LabTestVariableOptions { get; set; } = new List<LabTestVariableOption>();
    }

    public enum ResultType
    {
        Numeric = 1,
        Text = 2,
        Option = 3,
        Boolean = 4
    }

    public class LabTestVariableOption : BaseEntity
    {
        public long LabTestVariableId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public LabTestVariable LabTestVariable { get; set; }
    }
}
