using ERP.Core.Provider;
using ERP.Mediator.Mediator.ServiceAccount.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.ServiceAccount.Handler
{
    public class SaveServiceAccountHandler : IRequestHandler<SaveServiceAccountCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveServiceAccountHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<int> Handle(SaveServiceAccountCommand request, CancellationToken cancellationToken)
        {
            // 1. Fetch all currently active records for this ServiceTypeId
            var existingRecords = await unitOfWork.Repository<Entities.Models.ServiceAccount>()
                .FindAllAsync(x => x.ServiceTypeId == request.ServiceTypeId && x.IsActive == true);

            // 2. If no items are sent, soft-delete everything and return
            if (request.ServiceAccounts == null || !request.ServiceAccounts.Any())
            {
                foreach (var record in existingRecords)
                {
                    record.IsActive = false;
                    record.IsDelete = true;
                    record.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    record.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.ServiceAccount>().Update(record);
                }
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return 200;
            }

            // 3. Get IDs from incoming items (skip Id=0)
            var incomingItems = request.ServiceAccounts;
            var incomingIds = incomingItems.Where(i => i.Id != 0).Select(i => i.Id).ToList();
            var existingIds = existingRecords.Select(x => x.Id).ToList();

            // 4. Find IDs to delete (soft delete)
            var idsToDelete = existingIds.Except(incomingIds).ToList();

            foreach (var id in idsToDelete)
            {
                var record = existingRecords.FirstOrDefault(x => x.Id == id);
                if (record != null)
                {
                    record.IsActive = false;
                    record.IsDelete = true;
                    record.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    record.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.ServiceAccount>().Update(record);
                }
            }

            // 5. Process incoming items: update existing or add new
            foreach (var item in incomingItems)
            {
                if (item.Id != 0)
                {
                    // Try to update existing record
                    var existing = await unitOfWork.Repository<Entities.Models.ServiceAccount>()
                        .FindAsync(x => x.Id == item.Id && x.IsActive == true);

                    if (existing != null)
                    {
                        // Update all fields
                        existing.ProjectId = item.ProjectId;
                        existing.PaymentModeId = item.PaymentModeId;
                        existing.AccountType = item.AccountType;
                        existing.DebitAccountId = item.DebitAccountId;
                        existing.CreditAccountId = item.CreditAccountId;
                        existing.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        existing.ModifiedDate = DateTime.Now;

                        unitOfWork.Repository<Entities.Models.ServiceAccount>().Update(existing);
                    }
                    else
                    {
                        // ID not found – treat as new (fallback)
                        var entity = new Entities.Models.ServiceAccount
                        {
                            ServiceTypeId = request.ServiceTypeId,
                            ProjectId = item.ProjectId,
                            PaymentModeId = item.PaymentModeId,
                            AccountType = item.AccountType,
                            DebitAccountId = item.DebitAccountId,
                            CreditAccountId = item.CreditAccountId,
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now,
                            IsActive = true,
                            IsDelete = false
                        };
                        await unitOfWork.Repository<Entities.Models.ServiceAccount>().AddAsync(entity);
                    }
                }
                else // Id == 0
                {
                    // Insert new record
                    var entity = new Entities.Models.ServiceAccount
                    {
                        ServiceTypeId = request.ServiceTypeId,
                        ProjectId = item.ProjectId,
                        PaymentModeId = item.PaymentModeId,
                        AccountType = item.AccountType,
                        DebitAccountId = item.DebitAccountId,
                        CreditAccountId = item.CreditAccountId,
                        CreatedById = sessionProvider.Session.LoggedInUserId,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        IsDelete = false
                    };
                    await unitOfWork.Repository<Entities.Models.ServiceAccount>().AddAsync(entity);
                }
            }

            // 6. Save all changes
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return 200;
        }
    }
}