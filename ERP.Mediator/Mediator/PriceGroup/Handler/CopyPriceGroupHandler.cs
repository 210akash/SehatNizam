using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PriceGroup.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PriceGroup.Handler
{
    public class CopyPriceGroupHandler : IRequestHandler<CopyPriceGroupCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public CopyPriceGroupHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<CopyPriceGroupCommand, long>.Handle(CopyPriceGroupCommand request, CancellationToken cancellationToken)
        {
            var region = await unitOfWork.Repository<Entities.Models.PriceGroup>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id,null,null, "PriceGroupDetails");
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.PriceGroup>().GetAsync(x => x.Title.ToLower() == request.Title.ToLower() && x.IsActive == true && x.IsDelete == false);

            if (checkDuplicate.Count() == 0)
            {
                if (region != null)
                {
                    var user = sessionProvider.Session.LoggedInUserId;
                    var date = DateTime.Now;
                    var _priceGroup = mapper.Map<Entities.Models.PriceGroup>(region);
                    _priceGroup.Id = 0;
                    _priceGroup.Title = request.Title;
                    _priceGroup.Description = request.Description;
                    _priceGroup.CreatedById = user;
                    _priceGroup.CreatedDate = date;

                    foreach (var item in _priceGroup.PriceGroupDetails)
                    {
                        item.Id = 0;
                        item.CreatedById = user;
                        item.CreatedDate = date;
                        item.ModifiedById = null;
                        item.ModifiedDate = null;
                    }

                    unitOfWork.Repository<Entities.Models.PriceGroup>().Add(_priceGroup);
                    SaveChanges();
                }
                return 200;
            }
            else
            {
                return 409;
            }
        }
    }
}