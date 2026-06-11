using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.BloodBank.Donor.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Donor.Handler
{
    public class GetBloodDonorByIdHandler : IRequestHandler<GetBloodDonorByIdQuery, GetBloodDonor>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetBloodDonorByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetBloodDonor> Handle(GetBloodDonorByIdQuery request, CancellationToken cancellationToken)
        {
            var donor = await unitOfWork.Repository<Entities.Models.BloodDonor>()
                .GetFirstAsNoTrackingAsync(
                    x => x.Id == request.Id && x.IsActive == true,
                    null,
                    null,
                    "BloodGroupMaster,CreatedBy");

            return mapper.Map<GetBloodDonor>(donor);
        }
    }
}
