using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Migrations;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Appointment.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class SaveConsultationHandler : IRequestHandler<SaveConsultationCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public SaveConsultationHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<long> Handle(SaveConsultationCommand request, CancellationToken cancellationToken)
        {
            // 2️⃣ Check if appointment exists
            var appointment = await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetFirstAsync(x => x.Id == request.AppointmentId);
            if (appointment != null)
            {
                // 21 Check if Consultation exists
                var Consultation = await unitOfWork.Repository<Entities.Models.Consultation>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

                if (Consultation == null)
                {
                    var newConsultation = mapper.Map<Entities.Models.Consultation>(request);
                    newConsultation.CreatedById = sessionProvider.Session.LoggedInUserId;
                    newConsultation.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Consultation>().Add(newConsultation);
                    await unitOfWork.SaveChangesAsync();
                }
                else
                {
                    var updateConsultation = mapper.Map<Entities.Models.Consultation>(request);
                    updateConsultation.CreatedById = sessionProvider.Session.LoggedInUserId;
                    updateConsultation.CreatedDate = DateTime.Now;
                    updateConsultation.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    updateConsultation.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Consultation>().Update(updateConsultation);
                    await unitOfWork.SaveChangesAsync();
                }

                appointment.AppointmentStatusId = 15;
                unitOfWork.Repository<Entities.Models.Appointment>().Update(appointment);
                int check = await unitOfWork.SaveChangesAsync(cancellationToken);
                return 200;
            }
            else
            {
                return 404;
            }
        }

    }
}