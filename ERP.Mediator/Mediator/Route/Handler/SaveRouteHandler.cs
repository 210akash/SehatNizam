using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Route.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Route.Handler
{
    public class SaveRouteHandler : IRequestHandler<SaveRouteCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveRouteHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveRouteCommand, long>.Handle(SaveRouteCommand request, CancellationToken cancellationToken)
        {
            var route = await unitOfWork.Repository<Entities.Models.Route>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            //var checkDuplicate = await unitOfWork.Repository<Entities.Models.Route>().GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            //if (checkDuplicate.Count() == 0)
            //{
            if (route == null)
            {
                var _route = mapper.Map<Entities.Models.Route>(request);
                _route.CreatedById = sessionProvider.Session.LoggedInUserId;
                _route.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Route>().Add(_route);
                SaveChanges();
            }
            else
            {
                var _route = mapper.Map<Entities.Models.Route>(request);
                _route.CreatedById = route.CreatedById;
                _route.CreatedDate = route.CreatedDate;
                _route.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _route.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Route>().Update(_route);
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