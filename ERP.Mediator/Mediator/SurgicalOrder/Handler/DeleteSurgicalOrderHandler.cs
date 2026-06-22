using ERP.Core.Provider;
using ERP.Mediator.Mediator.SurgicalOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.SurgicalOrder.Handler
{
    public class DeleteSurgicalOrderHandler : IRequestHandler<DeleteSurgicalOrderQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteSurgicalOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteSurgicalOrderQuery request, CancellationToken cancellationToken)
        {
            var entity = await unitOfWork.Repository<Entities.Models.SurgicalOrder>()
                .GetFirstAsync(x => x.Id == request.Id && !x.IsDelete);

            if (entity == null)
                return false;

            entity.IsDelete = true;
            entity.IsActive = false;
            entity.DeleteDate = DateTime.Now;
            entity.ModifiedById = sessionProvider.Session.LoggedInUserId;
            entity.ModifiedDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.SurgicalOrder>().Update(entity);
            var result = await unitOfWork.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
    }
}
