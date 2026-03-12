using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Shop.Handler
{
    public class VerifyShopByIdHandler : IRequestHandler<VerifyShopByIdQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public VerifyShopByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }
        public async Task<long> Handle(VerifyShopByIdQuery request, CancellationToken cancellationToken)
        {
            var shop = await unitOfWork.Repository<Entities.Models.Shop>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            shop.IsVerified = true;
            shop.VerifiedDate = DateTime.Now;
            shop.VerifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Shop>().Update(shop);
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

    }
}
