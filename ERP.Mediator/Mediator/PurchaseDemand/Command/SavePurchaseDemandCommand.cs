using MediatR;
using System.Collections.Generic;
using System;

namespace ERP.Mediator.Mediator.PurchaseDemand.Command
{
    public class SavePurchaseDemandCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string RequestNo { get; set; }
        public DateTime RequestDate { get; set; }
        public long PriorityId { get; set; }
        public long LocationId { get; set; }
        public long IndentTypeId { get; set; }
        public long StatusId { get; set; }
        public string Remarks { get; set; }
        public virtual List<SavePurchaseDemandDetailCommand> PurchaseDemandDetail { get; set; }
    }

    public class SavePurchaseDemandDetailCommand
    {
        public long Id { get; set; }
        public long PurchaseDemandId { get; set; }
        public long ItemId { get; set; }
        public decimal DemandQty { get; set; }
        public DateTime RequiredDate { get; set; }
        public long DepartmentId { get; set; }
        public long ProjectId { get; set; }
        public string Description { get; set; }
    }
}
