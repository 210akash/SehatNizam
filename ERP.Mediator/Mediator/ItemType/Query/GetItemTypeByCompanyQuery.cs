using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.ItemType.Query
{
    public class GetItemTypeByCompanyQuery : IRequest<List<GetItemType>>
    {
        public GetItemTypeByCompanyQuery(long CompanyId)
        {
            this.CompanyId = CompanyId;
        }

        public long CompanyId { get; set; }
    }
}