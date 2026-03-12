using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Inspection.Query
{
    public class GetPendingIGPItemsQuery : IRequest<List<GetIGPDetails>>
    {
        public GetPendingIGPItemsQuery(long IGPId,long InspectionId)
        {
            this.IGPId = IGPId;
            this.InspectionId = InspectionId;
        }

        public long IGPId { get; set; }
        public long InspectionId { get; set; }
    }
}