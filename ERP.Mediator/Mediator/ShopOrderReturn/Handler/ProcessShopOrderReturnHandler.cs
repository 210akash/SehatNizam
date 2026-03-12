using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ShopOrderReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Handler
{
    public class ProcessShopOrderReturnHandler : IRequestHandler<ProcessShopOrderReturnQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessShopOrderReturnHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessShopOrderReturnQuery request, CancellationToken cancellationToken)
        {
            var ShopOrderReturn = await unitOfWork.Repository<Entities.Models.ShopOrderReturn>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            ShopOrderReturn.StatusId = 3;
            ShopOrderReturn.ModifiedDate = DateTime.Now;
            ShopOrderReturn.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.ShopOrderReturn>().Update(ShopOrderReturn);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
