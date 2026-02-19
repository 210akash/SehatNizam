using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Project.Query
{
    public class GetProjectByIdQuery : IRequest<GetProject>
    {
        public GetProjectByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}