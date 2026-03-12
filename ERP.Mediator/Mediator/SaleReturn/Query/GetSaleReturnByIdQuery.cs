using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.SaleReturn.Query
{
    public class GetSaleReturnByIdQuery : IRequest<GetSaleReturn>
    {
        public GetSaleReturnByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}