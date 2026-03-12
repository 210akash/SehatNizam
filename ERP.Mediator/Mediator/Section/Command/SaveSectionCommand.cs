using ERP.Mediator.Mediator.PurchaseOrder.Command;
using ERP.Mediator.Mediator.Row.Command;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Section.Command
{
    public class SaveSectionCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long RowId { get; set; }        
    }
}
