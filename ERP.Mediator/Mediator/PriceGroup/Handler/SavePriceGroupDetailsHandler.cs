using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
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
    public class SavePriceGroupDetailsHandler : IRequestHandler<SavePriceGroupDetailsCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SavePriceGroupDetailsHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SavePriceGroupDetailsCommand, long>.Handle(SavePriceGroupDetailsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var PriceGroup = await unitOfWork.Repository<Entities.Models.PriceGroup>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
                if (PriceGroup == null)
                {
                    //Group Not Created
                    return 410;
                }
                foreach (var item in request.GetProductGroupDetails)
                {
                    var PriceGroupDetails = await unitOfWork.Repository<Entities.Models.PriceGroupDetails>().GetFirstAsNoTrackingAsync(x => x.PriceGroupId == request.Id && x.ItemId == item.ItemId);
                    if (PriceGroupDetails == null)
                    {
                        //Create
                        Entities.Models.PriceGroupDetails lObjPriceGroupDetails = new Entities.Models.PriceGroupDetails();
                        lObjPriceGroupDetails.PriceGroupId = request.Id;
                        lObjPriceGroupDetails.ItemId = item.ItemId;
                        lObjPriceGroupDetails.RetailPrice = (decimal)item.RetailPrice;
                        lObjPriceGroupDetails.TradePrice = (decimal)item.TradePrice;
                        lObjPriceGroupDetails.DistributorPrice = (decimal)item.DistributorPrice;
                        lObjPriceGroupDetails.DistributorPromo = (decimal)item.DistributorPromo;
                        lObjPriceGroupDetails.NetDistributorPrice = (decimal)item.NetDistributorPrice;
                        unitOfWork.Repository<Entities.Models.PriceGroupDetails>().Add(lObjPriceGroupDetails);
                        SaveChanges();
                    }
                    else
                    {
                        //update
                        PriceGroupDetails.RetailPrice = (decimal)item.RetailPrice;
                        PriceGroupDetails.TradePrice = (decimal)item.TradePrice;
                        PriceGroupDetails.DistributorPrice = (decimal)item.DistributorPrice;
                        PriceGroupDetails.DistributorPromo = (decimal)item.DistributorPromo;
                        PriceGroupDetails.NetDistributorPrice = (decimal)item.NetDistributorPrice;
                        PriceGroupDetails.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        PriceGroupDetails.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<Entities.Models.PriceGroupDetails>().Update(PriceGroupDetails);
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
