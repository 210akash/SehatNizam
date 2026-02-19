using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.IndentType.Query
{
    public class GetIndentTypeByIdQuery : IRequest<GetIndentType>
    {
        public GetIndentTypeByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}