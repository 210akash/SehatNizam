using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Query
{
    public class GetGRNByIdQuery : IRequest<GetGRN>
    {
        public GetGRNByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}