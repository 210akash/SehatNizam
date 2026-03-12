using ERP.BusinessModels.ParameterVM;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.PrimaryOrder.Command
{
    public class CreateOrderCommand : IRequest<long>
    {
        public long? Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string DealershipAddress { get; set; }

        public long? DealershipId { get; set; }
        public GetDealership Dealership { get; set; }

        public List<CreateOrderItems> OrderItemsList { get; set; }
        public List<ImageUploadModel> OrderAttachments { get; set; }
    }
}
