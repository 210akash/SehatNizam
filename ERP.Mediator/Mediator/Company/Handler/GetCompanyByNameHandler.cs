using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Company.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Company.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetCompanyByNameQuery, List<GetCompany>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetCompany>> Handle(GetCompanyByNameQuery request, CancellationToken cancellationToken)
        {
            var company = await unitOfWork.Repository<Entities.Models.Company>().GetAsync(y => y.Name == request.name);
            var _company = mapper.Map<List<GetCompany>>(company);
            return _company;
        }
    }
}
