using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SubCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SubCategory.Handler
{
    public class GetAllSubCategoryHandler : IRequestHandler<GetAllSubCategoryQuery, Tuple<IEnumerable<GetSubCategory>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllSubCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetSubCategory>, long>> Handle(GetAllSubCategoryQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.SubCategory, bool>> predicate;

            Expression<Func<Entities.Models.SubCategory, object>>[] includes = {
                x => x.CreatedBy,
                x => x.Company,
                x => x.Category
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Store Manager") || roles.Contains("Store Issuer"))
            {
                predicate = x => x.IsActive == true && x.CompanyId == this.sessionProvider.Session.CompanyId
                && (request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower()))
                && (request.CategoryId == null || x.CategoryId == request.CategoryId);
            }
            else
            {
                predicate = x => x.IsActive == true
                && (request.Name == "" || request.Name == null || x.Name.ToLower().Contains(request.Name.ToLower()))
                && (request.CategoryId == null || x.CategoryId == request.CategoryId);
            }

            Expression<Func<Entities.Models.SubCategory, object>> OrderBy = null;
            Expression<Func<Entities.Models.SubCategory, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.SubCategory>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, null, includes);

            var SubCategory = mapper.Map<IEnumerable<GetSubCategory>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetSubCategory>, long>(SubCategory, entity.Item2);
        }
    }
}
