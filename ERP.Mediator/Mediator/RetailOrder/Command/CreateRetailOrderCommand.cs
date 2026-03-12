using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.RetailOrder.Command
{
    public class CreateRetailOrderCommand : IRequest<long>
    {
        public long? Id { get; set; }

        public string Reference { get; set; }
        public string Department { get; set; }
        public string Comments { get; set; }

        public List<GetRetailOrderItems> RetailOrderItemsList { get; set; }
    }
}
