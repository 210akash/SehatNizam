using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Rack.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Rack.Handler
{
    public class GetAllRackHandler : IRequestHandler<GetAllRackQuery, Tuple<IEnumerable<GetRack>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllRackHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetRack>, long>> Handle(GetAllRackQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Rack, bool>> predicate = x => x.IsActive == true
            && (string.IsNullOrEmpty(request.Name) || x.Name.ToLower().Contains(request.Name.ToLower()))
            ;

            Expression<Func<Entities.Models.Rack, object>>[] includes = {x => x.Company};

            Expression<Func<Entities.Models.Rack, object>> OrderBy = null;
            Expression<Func<Entities.Models.Rack, object>> OrderByDescending = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<Entities.Models.Rack>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDescending, null, includes);
            var Rack = mapper.Map<IEnumerable<GetRack>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetRack>, long>(Rack, entity.Item2);
        }
    }
}
