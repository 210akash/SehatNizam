using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Company.Query
{
    public class GetCompanyByIdQuery : IRequest<GetCompany>
    {
        public GetCompanyByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}