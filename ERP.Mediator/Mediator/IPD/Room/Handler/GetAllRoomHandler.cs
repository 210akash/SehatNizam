using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.Room.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Room.Handler
{
    public class GetAllRoomHandler : IRequestHandler<GetAllRoomQuery, Tuple<IEnumerable<GetRoom>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllRoomHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetRoom>, long>> Handle(GetAllRoomQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Room, bool>> predicate;

            Expression<Func<Entities.Models.Room, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Ward
            };

            predicate = x => x.IsActive == true
            && (x.Ward.ProjectId == sessionProvider.Session.SelectedWarehouseId)
            && (request.Code == "" || request.Code == null || x.Code.ToLower().Contains(request.Code.ToLower()))
            && (request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower()))
            && (request.WardId == null || x.WardId == request.WardId);

            Expression<Func<Entities.Models.Room, object>> OrderBy = null;
            Expression<Func<Entities.Models.Room, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Room>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var Room = mapper.Map<IEnumerable<GetRoom>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetRoom>, long>(Room, entity.Item2);
        }
    }
}
