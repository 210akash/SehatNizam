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
    public class SavePriceGroupHandler : IRequestHandler<SavePriceGroupCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SavePriceGroupHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SavePriceGroupCommand, long>.Handle(SavePriceGroupCommand request, CancellationToken cancellationToken)
        {
            var priceGroup = await unitOfWork.Repository<Entities.Models.PriceGroup>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.PriceGroup>().GetAsync(x => x.Title.ToLower() == request.Title.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (priceGroup == null)
                {
                    var _priceGroup = mapper.Map<Entities.Models.PriceGroup>(request);
                    _priceGroup.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _priceGroup.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.PriceGroup>().Add(_priceGroup);
                    SaveChanges();
                }
                else
                {
                    var _priceGroup = mapper.Map<Entities.Models.PriceGroup>(request);
                    _priceGroup.CreatedById = priceGroup.CreatedById;
                    _priceGroup.CreatedDate = priceGroup.CreatedDate;
                    _priceGroup.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _priceGroup.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.PriceGroup>().Update(_priceGroup);
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