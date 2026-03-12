using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Issuance.Query
{
    public class GetIssuanceByIdQuery : IRequest<GetIssuance>
    {
        public GetIssuanceByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}