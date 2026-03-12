using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseReturn.Query
{
    public class GetPurchaseReturnByIdQuery : IRequest<GetPurchaseReturn>
    {
        public GetPurchaseReturnByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}