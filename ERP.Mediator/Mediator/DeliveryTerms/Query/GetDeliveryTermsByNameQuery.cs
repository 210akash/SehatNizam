using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.DeliveryTerms.Query
{
    public class GetDeliveryTermsByNameQuery : IRequest<List<GetDeliveryTerms>>
    {
        public GetDeliveryTermsByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}