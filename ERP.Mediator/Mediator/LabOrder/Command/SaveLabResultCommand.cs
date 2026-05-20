using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.LabOrder.Command
{
    public class SaveLabResultCommand : IRequest<long>
    {
        public long LabOrderId { get; set; }

        public List<SaveLabResultDetailDto> Results { get; set; }
            = new();
    }

    public class SaveLabResultDetailDto
    {
        public long LabTestVariableId { get; set; }

        public string ResultValue { get; set; }
    }
}
