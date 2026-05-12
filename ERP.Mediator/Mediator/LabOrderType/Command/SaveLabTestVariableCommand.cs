using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.LabOrderType.Command
{
    public class SaveLabTestVariableCommand : IRequest<int>
    {
        public long LabOrderTypeId { get; set; }
        public List<LabTestVariableDto> Variables { get; set; }
    }

    public class LabTestVariableDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal? MaleMin { get; set; }
        public decimal? MaleMax { get; set; }
        public decimal? FemaleMin { get; set; }
        public decimal? FemaleMax { get; set; }
        public bool HasGenderRange { get; set; }
    }
}
