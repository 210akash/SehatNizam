using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Referrer.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Referrer.Handler
{
    public class GetAllReferrerHandler : IRequestHandler<GetAllReferrerQuery, Tuple<IEnumerable<GetReferrer>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllReferrerHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetReferrer>, long>> Handle(GetAllReferrerQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Referrer, bool>> predicate = x => x.IsActive == true
             && (request.Name == "" || x.Name.Contains(request.Name))
             && (request.Hospital == "" || x.Hospital.Contains(request.Hospital))
             && (request.PhoneNo == "" || x.PhoneNo.Contains(request.PhoneNo));

            Expression<Func<Entities.Models.Referrer, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company
            };

            Expression<Func<Entities.Models.Referrer, object>> OrderBy = null;
            Expression<Func<Entities.Models.Referrer, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Referrer>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var Referrer = mapper.Map<IEnumerable<GetReferrer>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetReferrer>, long>(Referrer, entity.Item2);
        }
    }
}
