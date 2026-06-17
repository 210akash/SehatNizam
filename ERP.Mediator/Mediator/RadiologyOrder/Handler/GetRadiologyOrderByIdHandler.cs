using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.RadiologyOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.RadiologyOrder.Handler
{
    public class GetRadiologyOrderByIdHandler : IRequestHandler<GetRadiologyOrderByIdQuery, GetRadiologyOrder>
    {
        private const string Includes =
            "Status,RadiologyType,RadiologyType.Service,Appointment,Appointment.Patient,Appointment.Patient.PatientMaster,Appointment.Doctor,Appointment.Department,Appointment.Department.Company,Appointment.Referrer,RadiologyStudyResult,RadiologyStudyResult.Images";

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetRadiologyOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<GetRadiologyOrder> Handle(GetRadiologyOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.RadiologyOrder>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id && x.IsActive == true, null, null, Includes);

            return entity == null ? null : mapper.Map<GetRadiologyOrder>(entity);
        }
    }
}
