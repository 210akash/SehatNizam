using ERP.Core.Provider;
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
            var existing = await unitOfWork.Repository<Entities.Models.LabOrder>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (existing == null)
            {
                unitOfWork.Repository<Entities.Models.LabOrder>().Add(new Entities.Models.LabOrder
                {
                    AppointmentId = request.AppointmentId,
                    LabOrderTypeId = request.LabOrderTypeId,
                    StatusId = request.StatusId,
                    CreatedById = sessionProvider.Session.LoggedInUserId,
                    CreatedDate = DateTime.Now
                });
            }
            else
            {
                var entity = await unitOfWork.Repository<Entities.Models.LabOrder>().GetFirstAsync(x => x.Id == request.Id);
                if (entity == null) return 404;
                entity.AppointmentId = request.AppointmentId;
                entity.LabOrderTypeId = request.LabOrderTypeId;
                entity.StatusId = request.StatusId;
                entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
                entity.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.LabOrder>().Update(entity);
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return 200;
        }
    }
}
