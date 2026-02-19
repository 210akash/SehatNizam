using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.UOM.Query
{
    public class GetUOMByIdQuery : IRequest<GetUOM>
    {
        public GetUOMByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}