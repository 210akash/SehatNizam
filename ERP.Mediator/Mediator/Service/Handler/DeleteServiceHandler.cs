using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Service.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Service.Handler
{
    public class DeleteServiceHandler : IRequestHandler<DeleteServiceCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteServiceHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await unitOfWork.Repository<Entities.Models.Service>().GetFirstAsync(x=>x.Id  == request.Id);
            if (service == null)
            {
                return false;
            }

            service.IsDelete = true;
            service.IsActive = false;
            service.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
            service.DeleteDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.Service>().Update(service);
            await unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
