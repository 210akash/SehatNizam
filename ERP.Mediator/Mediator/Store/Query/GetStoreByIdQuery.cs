using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Store.Query
{
    public class GetStoreByIdQuery : IRequest<GetStore>
    {
        public GetStoreByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}