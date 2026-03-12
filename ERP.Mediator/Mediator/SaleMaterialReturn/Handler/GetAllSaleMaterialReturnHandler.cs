using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.SaleMaterialReturn.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterialReturn.Handler
{
    public class GetAllSaleMaterialReturnHandler : IRequestHandler<GetAllSaleMaterialReturnQuery, Tuple<IEnumerable<GetSaleMaterialReturn>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllSaleMaterialReturnHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetSaleMaterialReturn>, long>> Handle(GetAllSaleMaterialReturnQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.SaleMaterialReturn, bool>> predicate;
            Expression<Func<Entities.Models.SaleMaterialReturn, object>>[] includes = {
                x => x.CreatedBy,
                x => x.ModifiedBy,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.Project,
                x => x.CreatedBy.Department.Company,
                x => x.Status,
                x => x.SaleMaterial,
                x => x.SaleMaterial.Dealership,
                x => x.SaleMaterialReturnDetail.Where(y => y.IsActive == true), // Keep only active details
            };

            List<string> thenIncludes = new()
            {
                "SaleMaterialReturnDetail",
                "SaleMaterialReturnDetail.SaleMaterialDetail",
                "SaleMaterialReturnDetail.SaleMaterialDetail.Item"
            };

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Gate Clerk"))
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.DealershipId ==  0 || x.SaleMaterial.DealershipId == request.DealershipId)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.DealershipId == 0 || x.SaleMaterial.DealershipId == request.DealershipId)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.SaleMaterialReturn, object>> OrderBy = null;
            Expression<Func<Entities.Models.SaleMaterialReturn, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.SaleMaterialReturn>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var SaleMaterialReturn = mapper.Map<IEnumerable<GetSaleMaterialReturn>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetSaleMaterialReturn>, long>(SaleMaterialReturn, entity.Item2);
        }
    }
}
