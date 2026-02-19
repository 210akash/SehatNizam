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
using ERP.Mediator.Mediator.IndentRequest.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IndentRequest.Handler
{
    public class GetAllIndentRequestHandler : IRequestHandler<GetAllIndentRequestQuery, Tuple<IEnumerable<GetIndentRequest>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllIndentRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetIndentRequest>, long>> Handle(GetAllIndentRequestQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.IndentRequest, bool>> predicate;

            Expression<Func<Entities.Models.IndentRequest, object>>[] includes = {
                x => x.CreatedBy,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.Department,
                x => x.Department.Company,
                x => x.Status,
                x => x.Store,
                x => x.IndentType,
                x => x.IndentRequestDetail.Where(y => y.IsActive == true)  // Apply IsActive filter to the include
             };

            List<string> thenIncludes = new();
            thenIncludes.Add("IndentRequestDetail.Item");
            thenIncludes.Add("IndentRequestDetail.Item.UOM");

            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Manager"))
            {
                predicate = x => x.IsActive == true && x.DepartmentId == this.sessionProvider.Session.DepartmentId
                      && x.StatusId == request.StatusId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else if(roles.Contains("Assistant"))
            {
                predicate = x => x.IsActive == true && x.DepartmentId == this.sessionProvider.Session.DepartmentId
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

            Expression<Func<Entities.Models.IndentRequest, object>> OrderBy = null;
            Expression<Func<Entities.Models.IndentRequest, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.IndentRequest>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);
            var IndentRequest = mapper.Map<IEnumerable<GetIndentRequest>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetIndentRequest>, long>(IndentRequest, entity.Item2);
        }
    }
}
