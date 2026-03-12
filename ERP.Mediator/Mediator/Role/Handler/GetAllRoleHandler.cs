using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Role.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Role.Handler
{
    public class GetAllRoleHandler : IRequestHandler<GetAllRoleQuery, Tuple<IEnumerable<GetRoles>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllRoleHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetRoles>, long>> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<AspNetRoles, bool>> predicate = x => x.IsActive == true
            && (!string.IsNullOrWhiteSpace(request.Name) ? x.Name.Contains(request.Name) : true)
            && x.Name != "KSS"
            && x.Name != "Retailer"
            && x.Name != "Accounts"
            ;

            Expression<Func<AspNetRoles, object>>[] includes = {
            };

            Expression<Func<AspNetRoles, object>> OrderBy = null;
            Expression<Func<AspNetRoles, object>> OrderByDesc = x => x.ModifiedDate ?? x.CreatedDate;

            var entity = unitOfWork.Repository<AspNetRoles>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);
            var role = mapper.Map<IEnumerable<GetRoles>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetRoles>, long>(role, entity.Item2);
        }
    }
}
