using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PriceGroup.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PriceGroup.Handler
{
    public class DeletePriceGroupHandler : IRequestHandler<DeletePriceGroupQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeletePriceGroupHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeletePriceGroupQuery request, CancellationToken cancellationToken)
        {
            var _priceGroup = await unitOfWork.Repository<Entities.Models.PriceGroup>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            _priceGroup.IsDelete = true;
            _priceGroup.IsActive = false;
            _priceGroup.ModifiedDate = DateTime.Now;
            _priceGroup.DeleteDate = DateTime.Now;
            _priceGroup.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.PriceGroup>().Update(_priceGroup);
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
