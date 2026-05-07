using ERP.Core.Provider;
using ERP.Mediator.Mediator.LabOrderType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.LabOrderType.Handler
{
    public class DeleteLabOrderTypeHandler : IRequestHandler<DeleteLabOrderTypeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteLabOrderTypeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteLabOrderTypeQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.LabOrderType>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            if (entity == null)
            {
                return false;
            }

            entity.IsDelete = true;
            entity.IsActive = false;
            entity.DeleteDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            entity.ModifiedById = sessionProvider.Session.LoggedInUserId;

            unitOfWork.Repository<Entities.Models.LabOrderType>().Update(entity);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
