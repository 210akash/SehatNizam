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
            var PurchaseDemand = await unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            PurchaseDemand.IsDelete = true;
            PurchaseDemand.IsActive = false;
            PurchaseDemand.DeleteDate = DateTime.Now;
            PurchaseDemand.ModifiedDate = DateTime.Now;
            PurchaseDemand.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.PurchaseDemand>().Update(PurchaseDemand);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
