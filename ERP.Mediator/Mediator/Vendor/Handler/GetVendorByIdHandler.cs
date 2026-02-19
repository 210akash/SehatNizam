using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Vendor.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Vendor.Handler
{
    public class GetCommunicationModeByIdHandler : IRequestHandler<GetVendorByIdQuery, GetVendor>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetCommunicationModeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetVendor> Handle(GetVendorByIdQuery request, CancellationToken cancellationToken)
        {
            var vendor = await unitOfWork.Repository<Entities.Models.Vendor>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            var _vendor = mapper.Map<GetVendor>(vendor);
            return _vendor;
        }
    }
}
