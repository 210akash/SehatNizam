using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Project.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Project.Handler
{
    public class GetAllProjectHandler : IRequestHandler<GetAllProjectQuery, Tuple<IEnumerable<GetProject>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;
        public GetAllProjectHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetProject>, long>> Handle(GetAllProjectQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Project, bool>> predicate = x => x.IsActive == true &&
            x.CompanyId == sessionProvider.Session.CompanyId;

            Expression<Func<Entities.Models.Project, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company
            };

            Expression<Func<Entities.Models.Project, object>> OrderBy = null;
            Expression<Func<Entities.Models.Project, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Project>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var Project = mapper.Map<IEnumerable<GetProject>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetProject>, long>(Project, entity.Item2);
        }
    }
}
