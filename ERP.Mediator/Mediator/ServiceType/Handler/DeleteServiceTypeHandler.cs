using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ServiceType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ServiceType.Handler
{
    public class DeleteServiceTypeHandler : IRequestHandler<DeleteServiceTypeCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteServiceTypeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteServiceTypeCommand request, CancellationToken cancellationToken)
        {
            var ServiceType = await unitOfWork.Repository<Entities.Models.ServiceType>().GetFirstAsync(x=>x.Id  == request.Id);
            if (ServiceType == null)
            {
                return false;
            }

            ServiceType.IsDelete = true;
            ServiceType.IsActive = false;
            ServiceType.ModifiedById = this.sessionProvider.Session.LoggedInUserId;
            ServiceType.DeleteDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.ServiceType>().Update(ServiceType);
            await unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
