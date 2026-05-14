using ERP.Entities.Models;
using System.Collections.Generic;

namespace ERP.BusinessModels.ResponseVM
{
    public class GetLabTestVariable
    {
        public long Id { get; set; }
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
        public GetLabOrderType LabOrderType { get; set; }
        public List<GetLabTestVariableOption> LabTestVariableOptions { get; set; }
    }
  
}
