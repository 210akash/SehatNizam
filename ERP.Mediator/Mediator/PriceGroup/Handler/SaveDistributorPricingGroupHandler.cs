using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PrimaryOrder.Command;
using ERP.Mediator.Mediator.PriceGroup.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.PriceGroup.Handler
{
    public class SaveDistributorPricingGroupHandler : IRequestHandler<SaveDistributorPricingGroupCommand, long>
    {

        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveDistributorPricingGroupHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveDistributorPricingGroupCommand, long>.Handle(SaveDistributorPricingGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var ActiveDistributorPriceGroup = await unitOfWork.Repository<DistributorPriceGroup>().GetAsync(x => x.PriceGroupId == request.GroupId && x.IsActive == true && x.IsDelete == false);
                if (ActiveDistributorPriceGroup.Count() > 0)
                {
                    foreach (var item in ActiveDistributorPriceGroup)
                    {
                        item.IsActive = false;
                        item.IsDelete = true;
                        item.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        item.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<Entities.Models.DistributorPriceGroup>().Update(item);
                        SaveChanges();
                    }
                }

                foreach (var item in request.GetAllDistributorByGroupId)
                {
                    if (item.IsSelected == true && item.IsOccupiedInOtherGroup == false)
                    {
                        Entities.Models.DistributorPriceGroup lObjDistributorPriceGroup = new Entities.Models.DistributorPriceGroup();
                        lObjDistributorPriceGroup.DealershipId = item.DealershipId;
                        lObjDistributorPriceGroup.PriceGroupId = request.GroupId;
                        unitOfWork.Repository<Entities.Models.DistributorPriceGroup>().Add(lObjDistributorPriceGroup);
                        SaveChanges();
                    }
                }
                return 200;
            }
            catch (Exception ex)
            {
                return 409;
            }
        }
    }
}
