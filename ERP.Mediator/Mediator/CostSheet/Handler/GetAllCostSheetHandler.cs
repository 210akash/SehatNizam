using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.CostSheet.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.CostSheet.Handler
{
    public class GetAllCostSheetHandler : IRequestHandler<GetAllCostSheetQuery, Tuple<IEnumerable<GetCostSheet>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllCostSheetHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetCostSheet>, long>> Handle(GetAllCostSheetQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.CostSheet, bool>> predicate;

            Expression<Func<Entities.Models.CostSheet, object>>[] includes = {
                x => x.CreatedBy,
                x => x.CreatedBy.Department.Company,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.Item.Company,
                x => x.Status,
                x => x.CostSheetDetail.Where(y => y.IsActive == true)  // Apply IsActive filter to the include
             };

            List<string> thenIncludes = new();
            thenIncludes.Add("CostSheetDetail.Item");
            thenIncludes.Add("CostSheetDetail.Item.UOM");

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Store Manager"))
            {
                predicate = x => x.IsActive == true && x.Item.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.ItemId == null || x.ItemId == request.ItemId)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if(roles.Contains("Store Issuer"))
            {
                predicate = x => x.IsActive == true && x.Item.CompanyId == this.sessionProvider.Session.CompanyId
                      && x.StatusId == request.StatusId
                     // && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.ItemId == null || x.ItemId == request.ItemId)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.ItemId == null || x.ItemId == request.ItemId)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.CostSheet, object>> OrderBy = null;
            Expression<Func<Entities.Models.CostSheet, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.CostSheet>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);
            var CostSheet = mapper.Map<IEnumerable<GetCostSheet>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetCostSheet>, long>(CostSheet, entity.Item2);
        }
    }
}
