using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.GST.Query
{
    public class GetGSTByIdQuery : IRequest<GetGST>
    {
        public GetGSTByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}