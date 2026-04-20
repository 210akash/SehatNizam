using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Notification.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Notification.Handler
{
    public class DeleteNotificationHandler : IRequestHandler<DeleteNotificationCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteNotificationHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await unitOfWork.Repository<Entities.Models.Notification>().GetFirstAsync(x => x.Id == request.Id);
            
            if (notification == null)
            {
                return false;
            }

            notification.IsDelete = true;
            notification.IsActive = false;
            notification.DeleteDate = DateTime.Now;
            notification.ModifiedById = sessionProvider.Session.LoggedInUserId;
            notification.ModifiedDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.Notification>().Update(notification);
            unitOfWork.SaveChanges();

            return true;
        }
    }
}
