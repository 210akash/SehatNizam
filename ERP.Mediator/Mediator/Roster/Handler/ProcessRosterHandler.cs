using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Roster.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Roster.Handler
{
    public class ProcessRosterHandler : IRequestHandler<ProcessRosterQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ProcessRosterHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ProcessRosterQuery request, CancellationToken cancellationToken)
        {
            var Roster = await unitOfWork.Repository<Entities.Models.Roster>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Roster.StatusId = 2;
            Roster.ModifiedDate = DateTime.Now;
            Roster.ModifiedById = sessionProvider.Session.LoggedInUserId;

            Roster.ProcessedDate = DateTime.Now;
            Roster.ProcessedById = sessionProvider.Session.LoggedInUserId;

            unitOfWork.Repository<Entities.Models.Roster>().Update(Roster);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
