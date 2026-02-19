using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.AccountFlow.Command
{
    public class SaveAccountFlowCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
