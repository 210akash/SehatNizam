using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Company.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Company.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetCompanyByIdQuery, GetCompany>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetCompany> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var company = await unitOfWork.Repository<Entities.Models.Company>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _company = mapper.Map<GetCompany>(company);
            return _company;
        }
    }
}
