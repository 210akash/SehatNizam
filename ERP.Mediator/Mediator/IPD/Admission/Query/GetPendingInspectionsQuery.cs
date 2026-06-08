using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.GRN.Query
{
    public class GetPendingInspectionsQuery : IRequest<List<GetInspection>>
    {
        public GetPendingInspectionsQuery(long InspectionId)
        {
            this.InspectionId = InspectionId;
        }

        public long InspectionId { get; set; }
    }
}