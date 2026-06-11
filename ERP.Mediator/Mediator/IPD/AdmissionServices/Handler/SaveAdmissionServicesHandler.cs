using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.IPD.AdmissionServices.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.IPD.AdmissionServices.Handler
{
    public class SaveAdmissionServicesHandler : IRequestHandler<SaveAdmissionServicesCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAdmissionServicesHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveAdmissionServicesCommand, long>.Handle(SaveAdmissionServicesCommand request, CancellationToken cancellationToken)
        {
            var Admission = await unitOfWork.Repository<Entities.Models.Admission>().GetFirstAsNoTrackingAsync(x => x.Id == request.AdmissionId && x.IsActive == true);
            var Service = await unitOfWork.Repository<Entities.Models.Service>().GetFirstAsNoTrackingAsync(x => x.Id == request.ServiceId && x.IsActive == true, null, null, "ServiceType");

            if (Admission == null || Service == null)
            {
                return 404;
            }

            if (Service.ServiceType.Name == "Laboratory")
            {
                var LaborderType = await unitOfWork.Repository<Entities.Models.LabOrderType>()
                    .GetFirstAsNoTrackingAsync(x => x.ServiceId == Service.Id && x.IsActive == true);

                Entities.Models.LabOrder labOrder = new Entities.Models.LabOrder
                {
                    AppointmentId = Admission.AppointmentId,
                    LabOrderTypeId = LaborderType.Id,
                    StatusId = 1,
                    CreatedById = sessionProvider.Session.LoggedInUserId,
                    CreatedDate = DateTime.Now
                };

                unitOfWork.Repository<Entities.Models.LabOrder>().Add(labOrder);
            }
            else if (Service.ServiceType.Name == "Radiology")
            {
                var RadiologyType = await unitOfWork.Repository<Entities.Models.RadiologyType>()
                   .GetFirstAsNoTrackingAsync(x => x.ServiceId == Service.Id && x.IsActive == true);

                Entities.Models.RadiologyOrder radiologyOrder = new Entities.Models.RadiologyOrder
                {
                    AppointmentId = Admission.AppointmentId,
                    RadiologyTypeId = RadiologyType.Id,
                    StatusId = 1,
                    CreatedById = sessionProvider.Session.LoggedInUserId,
                    CreatedDate = DateTime.Now
                };

                unitOfWork.Repository<Entities.Models.RadiologyOrder>().Add(radiologyOrder);
            }

            var payment = new AppointmentPayment
            {
                AppointmentId = Admission.AppointmentId,
                VisitFee = Service.BasePrice,
                Discount = request.Discount,
                TotalPayable = Service.BasePrice - request.Discount,
                PaymentModeId = request.PaymentModeId,
                ServiceId = request.ServiceId,
                PaymentDate = DateTime.Now,
                PaymentStatusId = request.PaymentStatusId,
                CreatedById = sessionProvider.Session.LoggedInUserId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsDelete = false
            };

            await unitOfWork.Repository<AppointmentPayment>().AddAsync(payment);
            SaveChanges();
            return 200;
        }
    }
}