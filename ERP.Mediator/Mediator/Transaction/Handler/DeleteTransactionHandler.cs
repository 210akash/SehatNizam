using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Transaction.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Handler
{
    public class DeleteTransactionHandler : IRequestHandler<DeleteTransactionQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteTransactionHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteTransactionQuery request, CancellationToken cancellationToken)
        {
            var Transaction = await unitOfWork.Repository<Entities.Models.Transaction>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Transaction.IsDelete = true;
            Transaction.IsActive = false;
            Transaction.DeleteDate = DateTime.Now;
            Transaction.ModifiedDate = DateTime.Now;
            Transaction.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Transaction>().Update(Transaction);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
