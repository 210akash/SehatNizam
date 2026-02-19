using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PurchaseDemand.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PurchaseDemand.Handler
{
    public class GetAllPurchaseDemandHandler : IRequestHandler<GetAllPurchaseDemandQuery, Tuple<IEnumerable<GetPurchaseDemand>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllPurchaseDemandHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetPurchaseDemand>, long>> Handle(GetAllPurchaseDemandQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.PurchaseDemand, bool>> predicate;

            Expression<Func<Entities.Models.PurchaseDemand, object>>[] includes = {
                x => x.CreatedBy,
                x => x.CreatedBy.Department.Company,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.Status,
                x => x.Store,
                x => x.IndentType,
                x => x.Location,
                x => x.Location.Company,
                x => x.Priority,
                x => x.PurchaseDemandDetail.Where(y => y.IsActive == true)  // Apply IsActive filter to the include
             };

            List<string> thenIncludes = new();
            thenIncludes.Add("PurchaseDemandDetail.Department");
            thenIncludes.Add("PurchaseDemandDetail.Project");
            thenIncludes.Add("PurchaseDemandDetail.Item");
            thenIncludes.Add("PurchaseDemandDetail.Item.UOM");

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Store Manager"))
            {
                predicate = x => x.IsActive == true
                      && x.StoreId == this.sessionProvider.Session.StoreId
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if(roles.Contains("Store Issuer"))
            {
                predicate = x => x.IsActive == true
                      && x.StoreId == this.sessionProvider.Session.StoreId
                      && x.StatusId == request.StatusId
                      && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                       && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.PurchaseDemand, object>> OrderBy = null;
            Expression<Func<Entities.Models.PurchaseDemand, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.PurchaseDemand>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);
            var PurchaseDemand = mapper.Map<IEnumerable<GetPurchaseDemand>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetPurchaseDemand>, long>(PurchaseDemand, entity.Item2);
        }
    }
}
