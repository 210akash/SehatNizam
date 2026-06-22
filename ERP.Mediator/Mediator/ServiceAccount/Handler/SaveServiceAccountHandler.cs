using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.ServiceAccount.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.ServiceAccount.Handler
{
    public class SaveServiceAccountHandler : IRequestHandler<SaveServiceAccountCommand, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public SaveServiceAccountHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<int> Handle(SaveServiceAccountCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.ServiceAccounts)
            {
                var existing = await unitOfWork.Repository<Entities.Models.ServiceAccount>()
                    .GetFirstAsync(x =>
                        x.ServiceId == request.ServiceId &&
                        x.ProjectId == item.ProjectId &&
                        x.AccountType == item.AccountType
                    );

                if (existing != null)
                {
                    existing.DebitAccountId = item.DebitAccountId;
                    existing.CreditAccountId = item.CreditAccountId;
                    existing.ModifiedDate = DateTime.Now;
                    existing.ModifiedById = sessionProvider.Session.LoggedInUserId;

                    unitOfWork.Repository<Entities.Models.ServiceAccount>().Update(existing);
                }
                else
                {
                    var entity = new Entities.Models.ServiceAccount
                    {
                        ServiceId = request.ServiceId,
                        ProjectId = item.ProjectId,
                        AccountType = item.AccountType,
                        DebitAccountId = item.DebitAccountId,
                        CreditAccountId = item.CreditAccountId,
                        CreatedById = sessionProvider.Session.LoggedInUserId,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };

                    await unitOfWork.Repository<Entities.Models.ServiceAccount>().AddAsync(entity);
                }
            }

            await unitOfWork.SaveChangesAsync();
            return 200;
        }

        public async Task<int> Handle_Old(SaveServiceAccountCommand request, CancellationToken cancellationToken)
        {
            // =========================
            // 1. VALIDATION
            // =========================
            if (request.ServiceId <= 0)
                return 400;

            if (request.ServiceAccounts == null)
                return 400;

            // =========================
            // 2. LOAD EXISTING DATA
            // =========================
            var existingList = await unitOfWork.Repository<Entities.Models.ServiceAccount>()
                .GetPagingWhereAsNoTrackingAsync(
                    x => x.ServiceId == request.ServiceId && x.IsActive && !x.IsDelete,
                    null, null, null, null, null)
                .Item1.ToListAsync();

            // =========================
            // 3. BUILD REQUEST KEYS (for comparison)
            // =========================
            var requestKeys = request.ServiceAccounts
                .Select(x => new { x.AccountType, x.DebitAccountId, x.CreditAccountId })
                .ToList();

            var existingKeys = existingList
                .Select(x => new { x.AccountType, x.DebitAccountId, x.CreditAccountId })
                .ToList();

            // =========================
            // 4. FIND DELETED ITEMS
            // =========================
            var deletedItems = existingList
                .Where(e => !requestKeys.Any(r =>
                    r.AccountType == e.AccountType &&
                    r.DebitAccountId == e.DebitAccountId &&
                    r.CreditAccountId == e.CreditAccountId))
                .ToList();

            foreach (var item in deletedItems)
            {
                item.IsActive = false;
                item.IsDelete = true;
                item.DeleteDate = DateTime.Now;
                item.ModifiedById = sessionProvider.Session.LoggedInUserId;

                unitOfWork.Repository<Entities.Models.ServiceAccount>().Update(item);
            }

            // =========================
            // 5. UPDATE OR INSERT
            // =========================
            foreach (var item in request.ServiceAccounts)
            {
                var existing = existingList.FirstOrDefault(x =>
                    x.AccountType == item.AccountType &&
                    x.DebitAccountId == item.DebitAccountId &&
                    x.CreditAccountId == item.CreditAccountId);

                if (existing != null)
                {
                    // UPDATE (only metadata)
                    existing.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    existing.ModifiedDate = DateTime.Now;
                    existing.IsActive = true;
                    existing.IsDelete = false;

                    unitOfWork.Repository<Entities.Models.ServiceAccount>().Update(existing);
                }
                else
                {
                    // INSERT NEW
                    var newItem = new Entities.Models.ServiceAccount
                    {
                        ServiceId = request.ServiceId,
                        AccountType = item.AccountType,
                        DebitAccountId = item.DebitAccountId,
                        CreditAccountId = item.CreditAccountId,
                        CreatedById = sessionProvider.Session.LoggedInUserId,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        IsDelete = false
                    };

                    await unitOfWork.Repository<Entities.Models.ServiceAccount>().AddAsync(newItem);
                }
            }

            // =========================
            // 6. SAVE
            // =========================
            await unitOfWork.SaveChangesAsync();

            return 200;
        }
    }
}
