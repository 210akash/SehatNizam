using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.UOM.Query
{
    public class GetUOMByCompanyQuery : IRequest<List<GetUOM>>
    {
        public GetUOMByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}