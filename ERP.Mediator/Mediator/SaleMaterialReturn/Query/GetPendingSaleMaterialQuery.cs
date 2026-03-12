using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Query
{
    public class GetPendingSaleMaterialQuery : IRequest<List<GetSaleMaterial>>
    {
        public GetPendingSaleMaterialQuery(long SaleMaterialId, string searchParam)
        {
            this.SaleMaterialId = SaleMaterialId;
            this.searchParam = searchParam; 
        }

        public long SaleMaterialId { get; set; }
        public string searchParam { get; set; }
    }
}