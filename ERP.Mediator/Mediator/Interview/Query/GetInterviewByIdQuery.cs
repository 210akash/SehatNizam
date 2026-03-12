using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Interview.Query
{
    public class GetInterviewByIdQuery : IRequest<GetInterview>
    {
        public GetInterviewByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}