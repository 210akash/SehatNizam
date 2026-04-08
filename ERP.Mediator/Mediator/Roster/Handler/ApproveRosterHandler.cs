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
    public class ApproveRosterHandler : IRequestHandler<ApproveRosterQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ApproveRosterHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(ApproveRosterQuery request, CancellationToken cancellationToken)
        {
            var Roster = await unitOfWork.Repository<Entities.Models.Roster>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Roster.StatusId = 3;
            Roster.ModifiedDate = DateTime.Now;
            Roster.ModifiedById = sessionProvider.Session.LoggedInUserId;
            Roster.ApprovedDate = DateTime.Now;
            Roster.ApprovedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Roster>().Update(Roster);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
