using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ShopType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopType.Handler
{
    public class DeleteShopTypeHandler : IRequestHandler<DeleteShopTypeQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteShopTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteShopTypeQuery request, CancellationToken cancellationToken)
        {
            if (!await unitOfWork.Repository<Entities.Models.Shop>().GetExistsAsync(y => y.ShopTypeId == request.Id && y.IsActive))
            {
                var shopType = await unitOfWork.Repository<Entities.Models.ShopType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
                shopType.IsDelete = true;
                shopType.IsActive = false;
                shopType.ModifiedDate = DateTime.Now;
                shopType.DeleteDate = DateTime.Now;
                shopType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                unitOfWork.Repository<Entities.Models.ShopType>().Update(shopType);
                var check = await unitOfWork.SaveChangesAsync();
                if (check > 0)
                {
                    return (long)ResponseStatus.OK;
                }
                else
                {
                    return (long)ResponseStatus.Error;
                }
            }
            else
                return (long)ResponseStatus.Conflict;
        }
    }
}
