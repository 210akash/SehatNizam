using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.GRN.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Handler
{
    public class GetPurchaseInvoiceCountHandler : IRequestHandler<GetPurchaseInvoiceCountQuery, Tuple<long, long, long, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public GetPurchaseInvoiceCountHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, long, long, long>> Handle(GetPurchaseInvoiceCountQuery request, CancellationToken cancellationToken)
        {
            string[] roles = this.sessionProvider.Session.Roles;
            Expression<Func<Entities.Models.GRN, bool>> predicate;
            Expression<Func<Entities.Models.GRN, object>>[] includes = {
                x => x.Inspection,
                x => x.Inspection.IGP,
                x => x.Inspection.IGP.PurchaseOrder,
                x => x.Inspection.IGP.PurchaseOrder.Vendor,
             };
            // Check if the current user's RoleId array contains the AccountOwnerRoleId
            if (roles.Contains("Accounts Manager") || roles.Contains("Accounts Assistant") || roles.Contains("Audit"))
            {
                predicate = x => x.IsActive == true && x.InvoiceNo != null
                             && x.StatusId == 3
                          && x.Inspection.IGP.PurchaseOrder.CompanyId == this.sessionProvider.Session.CompanyId
                          && x.ApprovedDate != null
                          && x.ApprovedDate >= request.FDate.Value
                          && x.ApprovedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.VendorId == null || x.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                          && (request.GRNCode == "" || x.Code.ToLower().Contains(request.GRNCode.Trim()))
                          && (request.Code == "" || x.InvoiceNo.ToLower().Contains(request.Code.Trim()));
            }
            else
            {
                predicate = x => x.IsActive == true && x.InvoiceNo != null
                         && x.StatusId == 3
                          && x.CreatedDate >= request.FDate.Value
                          && x.CreatedDate <= request.TDate.Value.AddDays(1).AddTicks(-1)
                          && (request.VendorId == null || x.Inspection.IGP.PurchaseOrder.VendorId == request.VendorId)
                          && (request.GRNCode == "" || x.Code.ToLower().Contains(request.GRNCode.Trim()))
                          && (request.Code == "" || x.InvoiceNo.ToLower().Contains(request.Code.Trim()));
            }

            var entity = unitOfWork.Repository<Entities.Models.GRN>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, null, null);
            int Created = entity.Item1.Count(item => item.InvoiceStatusId == 1);
            int ProcessedAudit = entity.Item1.Count(item => item.InvoiceStatusId == 2);
            int ProcessedFinance = entity.Item1.Count(item => item.InvoiceStatusId == 6);
            int Approved = entity.Item1.Count(item => item.InvoiceStatusId == 3);
            return new Tuple<long, long, long, long>(Created, ProcessedAudit, ProcessedFinance, Approved);
        }
    }
}
