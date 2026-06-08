using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.Bed.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Bed.Handler
{
    public class GetAllBedHandler : IRequestHandler<GetAllBedQuery, Tuple<IEnumerable<GetBed>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllBedHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetBed>, long>> Handle(GetAllBedQuery request, CancellationToken cancellationToken)
        {
            string[] roles = sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Bed, bool>> predicate;

            Expression<Func<Entities.Models.Bed, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Room,
                x => x.Room.Ward
            };


            predicate = x => x.IsActive == true && x.Room.Ward.ProjectId == sessionProvider.Session.SelectedWarehouseId
            && (request.BedNo == "" || request.BedNo == null || x.BedNo.ToLower().Contains(request.BedNo.ToLower()))
            && (request.RoomId == null || x.RoomId == request.RoomId);

            Expression<Func<Entities.Models.Bed, object>> OrderBy = null;
            Expression<Func<Entities.Models.Bed, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Bed>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var Bed = mapper.Map<IEnumerable<GetBed>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetBed>, long>(Bed, entity.Item2);
        }
    }
}
