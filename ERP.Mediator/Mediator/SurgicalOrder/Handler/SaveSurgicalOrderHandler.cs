using ERP.Core.Provider;
using ERP.Mediator.Mediator.SurgicalOrder.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.SurgicalOrder.Handler
{
    public class SaveSurgicalOrderHandler : IRequestHandler<SaveSurgicalOrderCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveSurgicalOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SaveSurgicalOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.AppointmentId <= 0 || request.ServiceId <= 0 || request.SurgeonId == Guid.Empty)
                return 0;

            var appointment = await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.AppointmentId && x.IsActive && !x.IsDelete);
            if (appointment == null)
                return 0;

            var service = await unitOfWork.Repository<Entities.Models.Service>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.ServiceId && x.IsActive && !x.IsDelete && x.IsSurgical == true);
            if (service == null)
                return 0;

            if (request.Id > 0)
            {
                var entity = await unitOfWork.Repository<Entities.Models.SurgicalOrder>()
                    .GetFirstAsync(x => x.Id == request.Id && !x.IsDelete);

                if (entity == null)
                    return 0;

                entity.AppointmentId = request.AppointmentId;
                entity.ServiceId = request.ServiceId;
                entity.SurgeonId = request.SurgeonId;
                entity.ScheduledDateTime = request.ScheduledDateTime;
                entity.StatusId = request.StatusId;
                entity.ClinicalNotes = request.ClinicalNotes;
                entity.CompletedDateTime = request.CompletedDateTime;
                entity.CancelledDateTime = request.CancelledDateTime;
                entity.CancellationReason = request.CancellationReason;
                entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
                entity.ModifiedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.SurgicalOrder>().Update(entity);
                var updated = await unitOfWork.SaveChangesAsync(cancellationToken);
                return updated > 0 ? entity.Id : 0;
            }

            var surgicalOrder = new Entities.Models.SurgicalOrder
            {
                AppointmentId = request.AppointmentId,
                ServiceId = request.ServiceId,
                SurgeonId = request.SurgeonId,
                ScheduledDateTime = request.ScheduledDateTime,
                StatusId = request.StatusId > 0 ? request.StatusId : 1,
                ClinicalNotes = request.ClinicalNotes,
                CreatedById = sessionProvider.Session.LoggedInUserId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsDelete = false
            };

            await unitOfWork.Repository<Entities.Models.SurgicalOrder>().AddAsync(surgicalOrder);
            var saved = await unitOfWork.SaveChangesAsync(cancellationToken);
            return saved > 0 ? surgicalOrder.Id : 0;
        }
    }
}
