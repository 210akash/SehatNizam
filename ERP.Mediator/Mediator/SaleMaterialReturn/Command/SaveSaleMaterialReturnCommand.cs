using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Command
{
    public class SaveSaleMaterialReturnCommand : IRequest<long>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long SaleMaterialId { get; set; }
        public long? ProjectId { get; set; }
        public long StatusId { get; set; }
        public string Remarks { get; set; }
        public List<SaveSaleMaterialReturnDetailCommand> SaleMaterialReturnDetail { get; set; }
    }

    public class SaveSaleMaterialReturnDetailCommand
    {
        public long Id { get; set; }
        public long SaleMaterialReturnId { get; set; }
        public decimal Quantity { get; set; }
        public long SaleMaterialDetailId { get; set; }
    }
}
