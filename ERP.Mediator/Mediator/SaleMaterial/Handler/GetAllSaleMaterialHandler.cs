using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SaleMaterial.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterial.Handler
{
    public class GetAllSaleMaterialHandler : IRequestHandler<GetAllSaleMaterialQuery, Tuple<IEnumerable<GetSaleMaterial>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllSaleMaterialHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetSaleMaterial>, long>> Handle(GetAllSaleMaterialQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.SaleMaterial, bool>> predicate;

            Expression<Func<Entities.Models.SaleMaterial, object>>[] includes = {
                x => x.CreatedBy,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.Status,
                x => x.Dealership,
                x => x.Company,
                x => x.Project,
                x => x.SaleMaterialDetail.Where(y => y.IsActive == true)  // Apply IsActive filter to the include
             };

            List<string> thenIncludes = new()
            {
                "SaleMaterialDetail.Item",
                "SaleMaterialDetail.Item.UOM"
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Purchase Manager"))
            {
                predicate = x => x.IsActive == true
                      && x.ProjectId == sessionProvider.Session.SelectedWarehouseId
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if(roles.Contains("Purchaser"))
            {
                predicate = x => x.IsActive == true
                        && x.ProjectId == sessionProvider.Session.SelectedWarehouseId
                      && x.StatusId == request.StatusId
                      && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                        && x.ProjectId == sessionProvider.Session.SelectedWarehouseId
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                       && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.SaleMaterial, object>> OrderBy = null;
            Expression<Func<Entities.Models.SaleMaterial, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.SaleMaterial>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);
            var SaleMaterial = mapper.Map<IEnumerable<GetSaleMaterial>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetSaleMaterial>, long>(SaleMaterial, entity.Item2);
        }
    }
}
