using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ShopType.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ShopType.Handler
{
    public class SaveShopTypeHandler : IRequestHandler<SaveShopTypeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveShopTypeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveShopTypeCommand, long>.Handle(SaveShopTypeCommand request, CancellationToken cancellationToken)
        {
            var shopType = await unitOfWork.Repository<Entities.Models.ShopType>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            
            if (shopType == null)
            {
                var _shopType = mapper.Map<Entities.Models.ShopType>(request);
                _shopType.CreatedById = sessionProvider.Session.LoggedInUserId;
                _shopType.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.ShopType>().Add(_shopType);
                SaveChanges();
            }
            else
            {
                var _shopType = mapper.Map<Entities.Models.ShopType>(request);
                _shopType.CreatedById = shopType.CreatedById;
                _shopType.CreatedDate = shopType.CreatedDate;
                _shopType.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _shopType.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.ShopType>().Update(_shopType);
                SaveChanges();
            }
            return 200;

        }
    }
}