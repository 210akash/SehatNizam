using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.TriageCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.TriageCategory.Handler
{
    public class GetAllTriageCategoryHandler : IRequestHandler<GetAllTriageCategoryQuery, Tuple<IEnumerable<GetTriageCategory>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllTriageCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetTriageCategory>, long>> Handle(GetAllTriageCategoryQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.TriageCategory, bool>> predicate;

            Expression<Func<Entities.Models.TriageCategory, object>>[] includes = {
                x => x.CreatedBy
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Accounts Manager") || roles.Contains("Accounts Assistant"))
            {
                predicate = x => x.IsActive == true
                &&(request.Name == "" || request.Name == null || x.Name == request.Name);
            }
            else
            {
                predicate = x => x.IsActive == true
                  && (request.Name == "" || request.Name == null || x.Name == request.Name);
            }

            Expression<Func<Entities.Models.TriageCategory, object>> OrderBy = null;
            Expression<Func<Entities.Models.TriageCategory, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.TriageCategory>()
                .GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var TriageCategory = mapper.Map<IEnumerable<GetTriageCategory>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetTriageCategory>, long>(TriageCategory, entity.Item2);
        }
    }
}
