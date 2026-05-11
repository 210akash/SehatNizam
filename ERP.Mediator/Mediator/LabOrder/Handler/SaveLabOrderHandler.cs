using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.LabOrder.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.LabOrder.Handler
{
    public class SaveLabOrderHandler : IRequestHandler<SaveLabOrderCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public SaveLabOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SaveLabOrderCommand request, CancellationToken cancellationToken)
        {
            var existing = await unitOfWork.Repository<Entities.Models.LabOrder>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            if (existing == null)
            {
                Entities.Models.LabOrder labOrder = new Entities.Models.LabOrder
                {
                    AppointmentId = request.AppointmentId,
                    LabOrderTypeId = request.LabOrderTypeId,
                    StatusId = request.StatusId,
                    ClinicalNotes = request.ClinicalNotes,
                    CreatedById = sessionProvider.Session.LoggedInUserId,
                    CreatedDate = DateTime.Now
                };

                unitOfWork.Repository<Entities.Models.LabOrder>().Add(labOrder);

                var check = await unitOfWork.SaveChangesAsync(cancellationToken);

                return check > 0 ? labOrder.Id : 0;
            }
            else
            {
                var entity = await unitOfWork.Repository<Entities.Models.LabOrder>()
                    .GetFirstAsync(x => x.Id == request.Id);

                if (entity == null)
                    return 0;

                entity.AppointmentId = request.AppointmentId;
                entity.LabOrderTypeId = request.LabOrderTypeId;
                entity.StatusId = request.StatusId;
                entity.ClinicalNotes = request.ClinicalNotes;
                entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
                entity.ModifiedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.LabOrder>().Update(entity);

                var check = await unitOfWork.SaveChangesAsync(cancellationToken);

                return check > 0 ? entity.Id : 0;
            }
        }
    }
}
