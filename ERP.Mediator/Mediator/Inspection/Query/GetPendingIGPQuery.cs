using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Inspection.Query
{
    public class GetPendingIGPsQuery : IRequest<List<GetDropDown>>
    {
        public GetPendingIGPsQuery(long IGPId)
        {
            this.IGPId = IGPId;
        }

        public long IGPId { get; set; }
    }
}