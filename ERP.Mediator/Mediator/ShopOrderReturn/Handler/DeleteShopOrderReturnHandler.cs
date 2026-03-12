using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ShopOrderReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopOrderReturn.Handler
{
    public class DeleteShopOrderReturnHandler : IRequestHandler<DeleteShopOrderReturnQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteShopOrderReturnHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteShopOrderReturnQuery request, CancellationToken cancellationToken)
        {
            // 1️  Grab the ShopOrderReturn *with* its details and keep it tracked
            var ShopOrderReturn = await unitOfWork.Repository<Entities.Models.ShopOrderReturn>().GetFirstAsync(y => y.Id == request.Id, null, null, "ShopOrderReturnDetails");

            if (ShopOrderReturn is null) return false;

            var now = DateTime.UtcNow;               // safer than Now on servers
            var userId = sessionProvider.Session.LoggedInUserId;

            ShopOrderReturn.IsDelete = true;
            ShopOrderReturn.IsActive = false;
            ShopOrderReturn.DeleteDate = now;
            ShopOrderReturn.ModifiedDate = now;
            ShopOrderReturn.ModifiedById = userId;

            // 3️  Push the same flags into every child row
            foreach (var d in ShopOrderReturn.ShopOrderReturnDetail)
            {
                d.IsDelete = true;
                d.IsActive = false;
                d.DeleteDate = now;
                d.ModifiedDate = now;
                d.ModifiedById = userId;
            }

            unitOfWork.Repository<Entities.Models.ShopOrderReturn>().Update(ShopOrderReturn);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
