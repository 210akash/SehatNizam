using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.GRN.Query
{
    public class GetPendingInspectionsItemsQuery : IRequest<List<GetInspectionDetail>>
    {
        public GetPendingInspectionsItemsQuery(long InspectionId, long GRNId)
        {
            this.InspectionId = InspectionId;
            this.GRNId = GRNId;
        }

        public long InspectionId { get; set; }
        public long GRNId { get; set; }
    }
}