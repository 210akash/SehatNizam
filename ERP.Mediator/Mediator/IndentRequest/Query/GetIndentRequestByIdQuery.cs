using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.IndentRequest.Query
{
    public class GetIndentRequestByIdQuery : IRequest<GetIndentRequest>
    {
        public GetIndentRequestByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}