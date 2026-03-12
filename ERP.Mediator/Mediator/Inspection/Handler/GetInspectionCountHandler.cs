using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Inspection.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Inspection.Handler
{
    public class GetInspectionCountHandler : IRequestHandler<GetInspectionCountQuery, Tuple<long, long, long, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetInspectionCountHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, long, long, long>> Handle(GetInspectionCountQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.Inspection, bool>> predicate;
            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Inspection"))
            {
                predicate = x => x.IsActive == true
                          && x.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }
            else
            {
                predicate = x => x.IsActive == true
                          && x.IGP.ProjectId == this.sessionProvider.Session.SelectedWarehouseId
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.Code == "" || x.Code.ToLower().Contains(request.Code));
            }


            var entity = unitOfWork.Repository<Entities.Models.Inspection>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);

            int Created = 0;
            if (roles.Contains("Purchase Manager"))
            {
                Created = entity.Item1.Count(item => item.StatusId == 1);
            }
            else if (roles.Contains("Purchaser"))
            {
                Created = entity.Item1.Count(item => item.StatusId == 1 && item.CreatedById == this.sessionProvider.Session.LoggedInUserId);
            }
            else
            {
                Created = entity.Item1.Count(item => item.StatusId == 1);
            }

            int Processed = entity.Item1.Count(item => item.StatusId == 2);
            int Approved = entity.Item1.Count(item => item.StatusId == 3);
            int Issued = entity.Item1.Count(item => item.StatusId == 20);
            return new Tuple<long, long, long, long>(Created, Processed, Approved, Issued);
        }
    }
}
