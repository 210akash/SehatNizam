using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Zone.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Zone.Handler
{
    public class SaveZoneHandler : IRequestHandler<SaveZoneCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveZoneHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveZoneCommand, long>.Handle(SaveZoneCommand request, CancellationToken cancellationToken)
        {
            var zone = await unitOfWork.Repository<Entities.Models.Zone>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            //var checkDuplicate = await unitOfWork.Repository<Entities.Models.Zone>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            //if (checkDuplicate.Count() == 0)
            //{
            if (zone == null)
            {
                var _zone = mapper.Map<Entities.Models.Zone>(request);
                _zone.CreatedById = sessionProvider.Session.LoggedInUserId;
                _zone.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Zone>().Add(_zone);
                SaveChanges();
            }
            else
            {
                var _zone = mapper.Map<Entities.Models.Zone>(request);
                _zone.CreatedById = zone.CreatedById;
                _zone.CreatedDate = zone.CreatedDate;
                _zone.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _zone.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Zone>().Update(_zone);
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