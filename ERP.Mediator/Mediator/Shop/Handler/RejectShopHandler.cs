using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Shop.Handler
{
    public class RejectShopHandler : IRequestHandler<RejectShopQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public RejectShopHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(RejectShopQuery request, CancellationToken cancellationToken)
        {
            var Shop = await unitOfWork.Repository<Entities.Models.Shop>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Shop.StatusId = 4;
            Shop.ModifiedDate = DateTime.Now;
            Shop.ModifiedById = sessionProvider.Session.LoggedInUserId;
            // Build log entry
            var logEntry = new StringBuilder();
            logEntry.AppendLine($"Date : {DateTime.Now:yyyy-MM-dd h:mm tt}");
            logEntry.AppendLine($"Status : Rejected");
            logEntry.AppendLine("Remarks :");

            if (!string.IsNullOrWhiteSpace(request.Remarks))
            {
                // Preserve multiline remarks
                logEntry.AppendLine(request.Remarks.Trim());
            }

            logEntry.AppendLine(); // blank line between entries

            // Append to previous remarks
            if (string.IsNullOrWhiteSpace(Shop.Remarks))
                Shop.Remarks = logEntry.ToString();
            else
                Shop.Remarks += logEntry.ToString();
            unitOfWork.Repository<Entities.Models.Shop>().Update(Shop);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
