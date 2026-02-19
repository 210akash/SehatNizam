using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Vendor.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Vendor.Handler
{
    public class GetcommunicationModeByNameHandler : IRequestHandler<GetVendorByNameQuery, List<GetVendor>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetcommunicationModeByNameHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetVendor>> Handle(GetVendorByNameQuery request, CancellationToken cancellationToken)
        {
            var vendor = await unitOfWork.Repository<Entities.Models.Vendor>().GetAsync(y => y.Name == request.name);
            var _vendor = mapper.Map<List<GetVendor>>(vendor);
            return _vendor;
        }
    }
}
