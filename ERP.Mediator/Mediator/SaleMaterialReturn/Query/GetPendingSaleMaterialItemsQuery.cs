using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Query
{
    public class GetPendingSaleMaterialItemsQuery : IRequest<List<GetSaleMaterialDetail>>
    {
        public GetPendingSaleMaterialItemsQuery(long SaleMaterialId, long SaleMaterialReturnId)
        {
            this.SaleMaterialId = SaleMaterialId;
            this.SaleMaterialReturnId = SaleMaterialReturnId;
        }

        public long SaleMaterialId { get; set; }
        public long SaleMaterialReturnId { get; set; }
    }
}