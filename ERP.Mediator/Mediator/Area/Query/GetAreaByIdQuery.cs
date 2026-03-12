using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Area.Query
{
    public class GetAreaByIdQuery : IRequest<GetArea>
    {
        public GetAreaByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}