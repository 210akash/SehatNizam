using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.BloodBank.Request.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.BloodBank.Request.Handler
{
    public class GetBloodRequestByIdHandler : IRequestHandler<GetBloodRequestByIdQuery, GetBloodRequest>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetBloodRequestByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetBloodRequest> Handle(GetBloodRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var bloodRequest = await unitOfWork.Repository<Entities.Models.BloodRequest>()
                .GetFirstAsNoTrackingAsync(
                    x => x.Id == request.Id && x.IsActive == true,
                    null,
                    null,
                    "Admission,Appointment,Appointment.Patient,Appointment.Patient.PatientMaster,Appointment.Doctor,Appointment.Department,BloodGroupMaster,BloodComponentType,CreatedBy");

            return mapper.Map<GetBloodRequest>(bloodRequest);
        }
    }
}
