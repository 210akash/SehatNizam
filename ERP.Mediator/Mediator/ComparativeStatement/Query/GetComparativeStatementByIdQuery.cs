using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.ComparativeStatement.Query
{
    public class GetComparativeStatementByIdQuery : IRequest<GetComparativeStatement>
    {
        public GetComparativeStatementByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}