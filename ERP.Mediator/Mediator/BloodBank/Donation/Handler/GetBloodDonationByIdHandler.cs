using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.BloodBank.Donation.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Donation.Handler
{
    public class GetBloodDonationByIdHandler : IRequestHandler<GetBloodDonationByIdQuery, GetBloodDonation>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetBloodDonationByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetBloodDonation> Handle(GetBloodDonationByIdQuery request, CancellationToken cancellationToken)
        {
            var donation = await unitOfWork.Repository<Entities.Models.BloodDonation>()
                .GetFirstAsNoTrackingAsync(
                    x => x.Id == request.Id && x.IsActive == true,
                    null,
                    null,
                    "BloodDonor,BloodDonor.BloodGroupMaster,BloodComponentType,BloodGroupMaster,CreatedBy,Appointment,Appointment.Patient,Appointment.Patient.PatientMaster");

            var result = mapper.Map<GetBloodDonation>(donation);

            if (donation != null)
            {
                var unit = await unitOfWork.Repository<Entities.Models.BloodUnit>()
                    .GetFirstAsNoTrackingAsync(
                        x => x.BloodDonationId == donation.Id && x.IsActive == true,
                        null,
                        null,
                        "BloodComponentType,BloodGroupMaster,BloodFridge,BloodRack");

                result.BloodUnit = mapper.Map<GetBloodUnit>(unit);
            }

            return result;
        }
    }
}
