using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Query
{
    public class GetSaleMaterialReturnByIdQuery : IRequest<GetSaleMaterialReturn>
    {
        public GetSaleMaterialReturnByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}