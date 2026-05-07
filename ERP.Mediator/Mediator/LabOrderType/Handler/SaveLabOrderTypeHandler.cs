using ERP.Core.Provider;
using ERP.Mediator.Mediator.LabOrderType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.LabOrderType.Handler
{
    public class SaveLabOrderTypeHandler : IRequestHandler<SaveLabOrderTypeCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveLabOrderTypeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SaveLabOrderTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.LabOrderType>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            if (entity == null)
            {
                var newEntity = new Entities.Models.LabOrderType
                {
                    Name = request.Name,
                    Description = request.Description,
                    CustomFieldsSchema = request.CustomFieldsSchema,
                    ServiceId = request.ServiceId,
                    CreatedById = sessionProvider.Session.LoggedInUserId,
                    CreatedDate = DateTime.Now
                };

                unitOfWork.Repository<Entities.Models.LabOrderType>().Add(newEntity);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return 200;
            }

            var updateEntity = await unitOfWork.Repository<Entities.Models.LabOrderType>()
                .GetFirstAsync(x => x.Id == request.Id);

            if (updateEntity == null)
            {
                return 404;
            }

            updateEntity.Name = request.Name;
            updateEntity.Description = request.Description;
            updateEntity.CustomFieldsSchema = request.CustomFieldsSchema;
            updateEntity.ServiceId = request.ServiceId;
            updateEntity.ModifiedById = sessionProvider.Session.LoggedInUserId;
            updateEntity.ModifiedDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.LabOrderType>().Update(updateEntity);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return 200;
        }
    }
}
