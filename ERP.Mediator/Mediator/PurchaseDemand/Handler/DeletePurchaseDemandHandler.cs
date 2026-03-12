using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PurchaseDemand.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Handler
{
    public class DeletePurchaseDemandHandler : IRequestHandler<DeletePurchaseDemandQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeletePurchaseDemandHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeletePurchaseDemandQuery request, CancellationToken cancellationToken)
        {
            // 1️  Grab the PurchaseDemand *with* its details and keep it tracked
            var PurchaseDemand = await unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetFirstAsync(y => y.Id == request.Id, null, null, "PurchaseDemandDetail");

            if (PurchaseDemand is null) return false;

            var now = DateTime.UtcNow;               // safer than Now on servers
            var userId = sessionProvider.Session.LoggedInUserId;

            PurchaseDemand.IsDelete = true;
            PurchaseDemand.IsActive = false;
            PurchaseDemand.DeleteDate = now;
            PurchaseDemand.ModifiedDate = now;
            PurchaseDemand.ModifiedById = userId;

            // 3️  Push the same flags into every child row
            foreach (var d in PurchaseDemand.PurchaseDemandDetail)
            {
                d.IsDelete = true;
                d.IsActive = false;
                d.DeleteDate = now;
                d.ModifiedDate = now;
                d.ModifiedById = userId;
            }

            unitOfWork.Repository<Entities.Models.PurchaseDemand>().Update(PurchaseDemand);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
