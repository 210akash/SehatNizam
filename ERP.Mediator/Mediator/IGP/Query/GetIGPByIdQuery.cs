using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.IGP.Query
{
    public class GetIGPByIdQuery : IRequest<GetIGP>
    {
        public GetIGPByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}