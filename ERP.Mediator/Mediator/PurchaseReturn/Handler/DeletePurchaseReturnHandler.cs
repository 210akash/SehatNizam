using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PurchaseReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseReturn.Handler
{
    public class DeletePurchaseReturnHandler : IRequestHandler<DeletePurchaseReturnQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeletePurchaseReturnHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeletePurchaseReturnQuery request, CancellationToken cancellationToken)
        {
            // 1️  Grab the PurchaseReturn *with* its details and keep it tracked
            var PurchaseReturn = await unitOfWork.Repository<Entities.Models.PurchaseReturn>().GetFirstAsync(y => y.Id == request.Id, null, null, "PurchaseReturnDetails");

            if (PurchaseReturn is null) return false;

            var now = DateTime.UtcNow;               // safer than Now on servers
            var userId = sessionProvider.Session.LoggedInUserId;

            PurchaseReturn.IsDelete = true;
            PurchaseReturn.IsActive = false;
            PurchaseReturn.DeleteDate = now;
            PurchaseReturn.ModifiedDate = now;
            PurchaseReturn.ModifiedById = userId;

            // 3️  Push the same flags into every child row
            foreach (var d in PurchaseReturn.PurchaseReturnDetail)
            {
                d.IsDelete = true;
                d.IsActive = false;
                d.DeleteDate = now;
                d.ModifiedDate = now;
                d.ModifiedById = userId;
            }

            unitOfWork.Repository<Entities.Models.PurchaseReturn>().Update(PurchaseReturn);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
