using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.DeliveryTerms.Query
{
    public class GetDeliveryTermsByCompanyQuery : IRequest<List<GetDeliveryTerms>>
    {
        public GetDeliveryTermsByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}