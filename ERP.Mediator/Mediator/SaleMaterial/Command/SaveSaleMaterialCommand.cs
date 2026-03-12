using MediatR;
using System.Collections.Generic;
using System;

namespace ERP.Mediator.Mediator.SaleMaterial.Command
{
    public class SaveSaleMaterialCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public DateTime? Date { get; set; }
        public string Remarks { get; set; }
        public long DealershipId { get; set; }
        public long ProjectId { get; set; }
        public long StatusId { get; set; }
        public virtual List<SaveSaleMaterialDetailCommand> SaleMaterialDetail { get; set; }
    }

    public class SaveSaleMaterialDetailCommand
    {
        public long Id { get; set; }
        public long SaleMaterialId { get; set; }
        public long ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}
