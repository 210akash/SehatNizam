using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.RetailOrder.Command
{
    public class ConfirmRetailOrderQuantityCommand : IRequest<long>
    {
        public long Id { get; set; }

        public List<GetOrderItems> RetailOrderItemsList { get; set; }
    }
}
