using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Inspection.Query
{
    public class GetInspectionByIdQuery : IRequest<GetInspection>
    {
        public GetInspectionByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}