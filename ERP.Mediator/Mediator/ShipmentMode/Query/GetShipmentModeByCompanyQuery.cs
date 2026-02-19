using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.ShipmentMode.Query
{
    public class GetShipmentModeByCompanyQuery : IRequest<List<GetShipmentMode>>
    {
        public GetShipmentModeByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}