using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Region.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Region.Handler
{
    public class SaveRegionHandler : IRequestHandler<SaveRegionCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveRegionHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveRegionCommand, long>.Handle(SaveRegionCommand request, CancellationToken cancellationToken)
        {
            var region = await unitOfWork.Repository<Entities.Models.Region>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            //var checkDuplicate = await unitOfWork.Repository<Entities.Models.Region>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            //if (checkDuplicate.Count() == 0)
            //{
            if (region == null)
            {
                var _region = mapper.Map<Entities.Models.Region>(request);
                _region.CreatedById = sessionProvider.Session.LoggedInUserId;
                _region.CompanyId = sessionProvider.Session.CompanyId;
                _region.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Region>().Add(_region);
                SaveChanges();
            }
            else
            {
                var _region = mapper.Map<Entities.Models.Region>(request);
                _region.CreatedById = region.CreatedById;
                _region.CreatedDate = region.CreatedDate;
                _region.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _region.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Region>().Update(_region);
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