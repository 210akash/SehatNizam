using MediatR;
using System.Collections.Generic;
using System;

namespace ERP.Mediator.Mediator.IndentRequest.Command
{
    public class SaveIndentRequestCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime RequiredDate { get; set; }
        public long DepartmentId { get; set; }
        public long StoreId { get; set; }
        public long IndentTypeId { get; set; }
        public long StatusId { get; set; }
        public virtual List<SaveIndentRequestDetailCommand> IndentRequestDetail { get; set; }
    }

    public class SaveIndentRequestDetailCommand
    {
        public long Id { get; set; }
        public long IndentRequestId { get; set; }
        public long ItemId { get; set; }
        public decimal Required { get; set; }
        public string Description { get; set; }
    }
}
