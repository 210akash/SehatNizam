using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Twilio.TwiML.Voice;

namespace ERP.Mediator.Mediator.Shop.Handler
{
    public class ApproveShopHandler : IRequestHandler<ApproveShopQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ApproveShopHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ApproveShopQuery request, CancellationToken cancellationToken)
        {
            var shop = await unitOfWork.Repository<Entities.Models.Shop>()
                .GetFirstAsNoTrackingAsync(y => y.Id == request.Id);

            // Update status
            shop.StatusId = 3;
            shop.IsVerified = true;
            shop.VerifiedDate = DateTime.Now;
            shop.VerifiedById = sessionProvider.Session.LoggedInUserId;

            // Build new log entry
            var logEntry = new StringBuilder();
            logEntry.AppendLine($"Date : {DateTime.Now:yyyy-MM-dd h:mm tt}");
            logEntry.AppendLine($"Status : Approved");
            logEntry.AppendLine("Remarks :");

            if (!string.IsNullOrWhiteSpace(request.Remarks))
            {
                logEntry.AppendLine(request.Remarks.Trim());
            }

            logEntry.AppendLine(); // blank line between entries

            // ✅ Append only the new log entry
            if (string.IsNullOrWhiteSpace(shop.Remarks))
                shop.Remarks = logEntry.ToString();
            else
                shop.Remarks += logEntry.ToString();

            unitOfWork.Repository<Entities.Models.Shop>().Update(shop);
            await unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
