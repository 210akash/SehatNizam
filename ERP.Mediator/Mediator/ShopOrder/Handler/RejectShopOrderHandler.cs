using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ShopOrder.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterial.Handler
{
    public class RejectShopOrderHandler : IRequestHandler<RejectShopOrderQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public RejectShopOrderHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(RejectShopOrderQuery request, CancellationToken cancellationToken)
        {
            var SaleMaterial = await unitOfWork.Repository<Entities.Models.ShopOrder>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            SaleMaterial.ShopOrderStatusId = 60;
            SaleMaterial.ModifiedDate = DateTime.Now;
            SaleMaterial.ModifiedById = request.UserId;
            SaleMaterial.Remarks = request.Remarks != "" ? SaleMaterial.Remarks + ", " + request.Remarks : SaleMaterial.Remarks;
            unitOfWork.Repository<Entities.Models.ShopOrder>().Update(SaleMaterial);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
