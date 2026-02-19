using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Store.Query
{
    public class GetStoreByCompanyQuery : IRequest<List<GetStore>>
    {
        public GetStoreByCompanyQuery(long CompanyId,bool FixedAsset)
        {
            this.CompanyId = CompanyId;
            this.FixedAsset = FixedAsset;
        }

        public long CompanyId { get; set; }
        public bool FixedAsset { get; set; }
    }
}