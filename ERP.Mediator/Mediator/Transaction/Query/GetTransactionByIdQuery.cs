using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Query
{
    public class GetTransactionByIdQuery : IRequest<GetTransaction>
    {
        public GetTransactionByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}