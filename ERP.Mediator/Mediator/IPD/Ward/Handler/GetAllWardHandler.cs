using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.Ward.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Ward.Handler
{
    public class GetAllWardHandler : IRequestHandler<GetAllWardQuery, Tuple<IEnumerable<GetWard>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllWardHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetWard>, long>> Handle(GetAllWardQuery request, CancellationToken cancellationToken)
        {
            string[] roles = sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Ward, bool>> predicate;

            Expression<Func<Entities.Models.Ward, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Project,
                x => x.Department
            };

       
                predicate = x => x.IsActive == true
                &&(request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower().Trim()))
                && x.ProjectId == sessionProvider.Session.SelectedWarehouseId;
           

            Expression<Func<Entities.Models.Ward, object>> OrderBy = null;
            Expression<Func<Entities.Models.Ward, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Ward>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var Ward = mapper.Map<IEnumerable<GetWard>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetWard>, long>(Ward, entity.Item2);
        }
    }
}
