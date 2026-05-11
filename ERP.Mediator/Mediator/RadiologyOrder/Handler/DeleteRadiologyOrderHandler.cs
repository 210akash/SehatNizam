using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.RadiologyOrder.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyOrder.Handler
{
    public class DeleteRadiologyOrderHandler : IRequestHandler<DeleteRadiologyOrderCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteRadiologyOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteRadiologyOrderCommand request, CancellationToken cancellationToken)
        {
            var RadiologyOrder = await unitOfWork.Repository<ERP.Entities.Models.RadiologyOrder>().GetFirstAsync(x => x.Id == request.Id);
            if (RadiologyOrder == null)
            {
                return false;
            }

            RadiologyOrder.IsDelete = true;
            RadiologyOrder.IsActive = false;
            RadiologyOrder.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
            RadiologyOrder.DeleteDate = DateTime.Now;

            unitOfWork.Repository<ERP.Entities.Models.RadiologyOrder>().Update(RadiologyOrder);
            await unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
