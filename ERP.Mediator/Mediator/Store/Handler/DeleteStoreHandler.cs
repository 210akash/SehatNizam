using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Store.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Store.Handler
{
    public class DeleteStoreHandler : IRequestHandler<DeleteStoreQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteStoreHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteStoreQuery request, CancellationToken cancellationToken)
        {
            var Store = await unitOfWork.Repository<Entities.Models.Store>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Store.IsDelete = true;
            Store.IsActive = false;
            Store.DeleteDate = DateTime.Now;
            Store.ModifiedDate = DateTime.Now;
            Store.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Store>().Update(Store);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
