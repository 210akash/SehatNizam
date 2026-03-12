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
using ERP.Mediator.Mediator.Issuance.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Issuance.Handler
{
    public class GetAllIssuanceHandler : IRequestHandler<GetAllIssuanceQuery, Tuple<IEnumerable<GetIssuance>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllIssuanceHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetIssuance>, long>> Handle(GetAllIssuanceQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Issuance, bool>> predicate;

            predicate = x => x.IsActive == true
                      && x.StatusId == request.StatusId
                      && x.ProjectId == sessionProvider.Session.SelectedWarehouseId
                      && x.CreatedDate >= request.FDate.Value
                      && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                      && (request.Code == "" || x.Code.ToLower().Contains(request.Code));

            Expression<Func<Entities.Models.Issuance, object>>[] includes = {
                x => x.IndentRequest,
                x => x.IndentRequest.Department,
                x => x.IndentRequest.Project,
                x => x.IndentRequest.Store,
                x => x.CreatedBy,
                x => x.CreatedBy.Department,
                x => x.CreatedBy.Department.Company,
                x => x.ProcessedBy,
                x => x.ApprovedBy,
                x => x.Status,
                x => x.Account,
                x => x.Project,
                x => x.IssuanceDetail.Where(y => y.IsActive == true)  // Apply IsActive filter to the include
             };

            List<string> thenIncludes = new()
            {
                "IssuanceDetail.CostSheet",
                "IssuanceDetail.IndentRequestDetail.Item",
                "IssuanceDetail.IndentRequestDetail.Item.UOM",
            };

            Expression<Func<Entities.Models.Issuance, object>> OrderBy = null;
            Expression<Func<Entities.Models.Issuance, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Issuance>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);
            var Issuance = mapper.Map<IEnumerable<GetIssuance>>(entity.Item1.ToList()).ToList();
            return new Tuple<IEnumerable<GetIssuance>, long>(Issuance, entity.Item2);
        }
    }
}
