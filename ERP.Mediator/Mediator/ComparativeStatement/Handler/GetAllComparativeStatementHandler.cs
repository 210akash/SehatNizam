using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ComparativeStatement.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.ComparativeStatement.Handler
{
    public class GetAllComparativeStatementHandler : IRequestHandler<GetAllComparativeStatementQuery, Tuple<IEnumerable<GetComparativeStatement>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllComparativeStatementHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetComparativeStatement>, long>> Handle(GetAllComparativeStatementQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.ComparativeStatement, bool>> predicate;
            Expression<Func<Entities.Models.ComparativeStatement, object>>[] includes = {
                x => x.CreatedBy,
                x => x.CreatedBy.Department.Company,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.Status,
                x => x.PurchaseDemand,
                x => x.ComparativeStatementDetail.Where(y => y.IsActive == true), // Keep only active details
            };

            List<string> thenIncludes = new();
            thenIncludes.Add("ComparativeStatementDetail.PurchaseDemandDetail");
            thenIncludes.Add("ComparativeStatementDetail.PurchaseDemandDetail.Item");
            thenIncludes.Add("ComparativeStatementDetail.PurchaseDemandDetail.Item.UOM");
            thenIncludes.Add("ComparativeStatementDetail.ComparativeStatementVendor.Vendor");

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Purchase Manager"))
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && x.ComparativeStatementDetail.Where(y => y.IsActive == true)
                      .Any(d => d.ComparativeStatementVendor.Any(v => v.IsActive == true))
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if (roles.Contains("Purchaser"))
            {
                predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.CreatedById == this.sessionProvider.Session.LoggedInUserId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && x.ComparativeStatementDetail.Where(y => y.IsActive == true)
                      .Any(d => d.ComparativeStatementVendor.Any(v => v.IsActive == true))
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

            Expression<Func<Entities.Models.ComparativeStatement, object>> OrderBy = null;
            Expression<Func<Entities.Models.ComparativeStatement, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.ComparativeStatement>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var ComparativeStatement = mapper.Map<IEnumerable<GetComparativeStatement>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetComparativeStatement>, long>(ComparativeStatement, entity.Item2);
        }
    }
}
