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

        public async Task<int> Handle(
     SaveServiceAccountCommand request,
     CancellationToken cancellationToken)
        {
            foreach (var project in request.ServiceAccounts)
            {
                foreach (var item in project.ServiceAccounts)
                {
                    var existing = await unitOfWork.Repository<Entities.Models.ServiceAccount>()
                        .GetFirstAsync(x =>
                            x.ServiceId == request.ServiceId &&
                            x.ProjectId == project.ProjectId &&
                            x.AccountType == item.AccountType);

                    if (existing != null)
                    {
                        existing.DebitAccountId = item.DebitAccountId;
                        existing.CreditAccountId = item.CreditAccountId;
                        existing.ModifiedDate = DateTime.Now;
                        existing.ModifiedById = sessionProvider.Session.LoggedInUserId;

                        unitOfWork.Repository<Entities.Models.ServiceAccount>()
                            .Update(existing);
                    }
                    else
                    {
                        var entity = new Entities.Models.ServiceAccount
                        {
                            ServiceId = request.ServiceId,
                            ProjectId = project.ProjectId,
                            AccountType = item.AccountType,
                            DebitAccountId = item.DebitAccountId,
                            CreditAccountId = item.CreditAccountId,
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now,
                            IsActive = true
                        };

                        await unitOfWork.Repository<Entities.Models.ServiceAccount>()
                            .AddAsync(entity);
                    }
                }
            }

            await unitOfWork.SaveChangesAsync();

            return 200;
        }
    }
}
