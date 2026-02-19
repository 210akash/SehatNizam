using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IGP.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IGP.Handler
{
    public class GetAllIGPHandler : IRequestHandler<GetAllIGPQuery, Tuple<IEnumerable<GetIGP>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllIGPHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetIGP>, long>> Handle(GetAllIGPQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.IGP, bool>> predicate;
            Expression<Func<Entities.Models.IGP, object>>[] includes = {
                x => x.CreatedBy,
                x => x.CreatedBy.Department.Company,
                x => x.Status,
                x => x.IGPDetails.Where(y => y.IsActive == true), // Keep only active details
                x => x.PurchaseOrder
            };

            List<string> thenIncludes = new();
            thenIncludes.Add("IGPDetails.PurchaseOrderDetail");
            thenIncludes.Add("IGPDetails.PurchaseOrderDetail.PurchaseDemandDetail");
            thenIncludes.Add("IGPDetails.PurchaseOrderDetail.PurchaseDemandDetail.Item");

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Purchase Manager"))
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      //&& x.IGPDetails.Where(y => y.IsActive == true)
                      //.Any(d => d.IGPVendor.Any(v => v.IsActive == true))
                      //&& (request.Code == "" || x.Code.ToLower().Contains(request.Code))
                      ;
            }
            else if (roles.Contains("Purchaser"))
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      //&& x.IGPDetails.Where(y => y.IsActive == true)
                      //.Any(d => d.IGPVendor.Any(v => v.IsActive == true))
                      //&& (request.Code == "" || x.Code.ToLower().Contains(request.Code))
                      ;
            }
            else
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                       && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }

            Expression<Func<Entities.Models.IGP, object>> OrderBy = null;
            Expression<Func<Entities.Models.IGP, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.IGP>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var IGP = mapper.Map<IEnumerable<GetIGP>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetIGP>, long>(IGP, entity.Item2);
        }
    }
}
