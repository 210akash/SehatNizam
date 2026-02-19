using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Priority.Query
{
    public class GetPriorityByIdQuery : IRequest<GetPriority>
    {
        public GetPriorityByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}