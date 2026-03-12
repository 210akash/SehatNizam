using MediatR;
using System.Collections.Generic;
using System;
using ERP.Entities.Models;

namespace ERP.Mediator.Mediator.Issuance.Command
{
    public class SaveIssuanceCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public long IndentRequestId { get; set; }
        public long? AccountId { get; set; }
        public long StatusId { get; set; }
        public string Remarks { get; set; }
        public virtual List<SaveIssuanceDetailCommand> IssuanceDetail { get; set; }
    }

    public class SaveIssuanceDetailCommand
    {
        public long Id { get; set; }
        public long IssuanceId { get; set; }
        public long IndentRequestDetailId { get; set; }
        public long? CostSheetId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}
