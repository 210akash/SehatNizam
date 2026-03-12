using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Area.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Area.Handler
{
    public class SaveAreaHandler : IRequestHandler<SaveAreaCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAreaHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveAreaCommand, long>.Handle(SaveAreaCommand request, CancellationToken cancellationToken)
        {
            var area = await unitOfWork.Repository<Entities.Models.Area>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            //var checkDuplicate = await unitOfWork.Repository<Entities.Models.Area>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            //if (checkDuplicate.Count() == 0)
            //{
            if (area == null)
            {
                var _area = mapper.Map<Entities.Models.Area>(request);
                _area.CreatedById = sessionProvider.Session.LoggedInUserId;
                _area.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Area>().Add(_area);
                SaveChanges();
            }
            else
            {
                var _area = mapper.Map<Entities.Models.Area>(request);
                _area.CreatedById = area.CreatedById;
                _area.CreatedDate = area.CreatedDate;
                _area.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _area.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Area>().Update(_area);
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