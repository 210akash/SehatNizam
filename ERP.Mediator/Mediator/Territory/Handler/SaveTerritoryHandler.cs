using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Territory.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Territory.Handler
{
    public class SaveTerritoryHandler : IRequestHandler<SaveTerritoryCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveTerritoryHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveTerritoryCommand, long>.Handle(SaveTerritoryCommand request, CancellationToken cancellationToken)
        {
            var territory = await unitOfWork.Repository<Entities.Models.Territory>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            //var checkDuplicate = await unitOfWork.Repository<Entities.Models.Territory>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            //if (checkDuplicate.Count() == 0)
            //{
            if (territory == null)
            {
                var _territory = mapper.Map<Entities.Models.Territory>(request);
                _territory.CreatedById = sessionProvider.Session.LoggedInUserId;
                _territory.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Territory>().Add(_territory);
                SaveChanges();
            }
            else
            {
                var _territory = mapper.Map<Entities.Models.Territory>(request);
                _territory.CreatedById = territory.CreatedById;
                _territory.CreatedDate = territory.CreatedDate;
                _territory.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _territory.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Territory>().Update(_territory);
                SaveChanges();
            }
            return 200;
            //}
            //else
            //{
            //    return 409;
            //}

        }
    }
}