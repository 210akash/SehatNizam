using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.Ward.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Ward.Handler
{
    public class DeleteWardHandler : IRequestHandler<DeleteWardQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public DeleteWardHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteWardQuery request, CancellationToken cancellationToken)
        {
            var Ward = await unitOfWork.Repository<Entities.Models.Ward>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Ward.IsDelete = true;
            Ward.IsActive = false;
            Ward.DeleteDate = DateTime.Now;
            Ward.ModifiedDate = DateTime.Now;
            Ward.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Ward>().Update(Ward);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
