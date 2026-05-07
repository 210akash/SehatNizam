using ERP.Core.Provider;
using ERP.Mediator.Mediator.LabOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.LabOrder.Handler
{
    public class DeleteLabOrderHandler : IRequestHandler<DeleteLabOrderQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public DeleteLabOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }
        public async Task<bool> Handle(DeleteLabOrderQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.LabOrder>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            if (entity == null) return false;
            entity.IsDelete = true;
            entity.IsActive = false;
            entity.DeleteDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;
            entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.LabOrder>().Update(entity);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
