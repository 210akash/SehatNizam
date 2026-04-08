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
    public class DeleteRosterHandler : IRequestHandler<DeleteRosterQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteRosterHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteRosterQuery request, CancellationToken cancellationToken)
        {
            var Roster = await unitOfWork.Repository<Entities.Models.Roster>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Roster.IsDelete = true;
            Roster.IsActive = false;
            Roster.DeleteDate = DateTime.Now;
            Roster.ModifiedDate = DateTime.Now;
            Roster.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Roster>().Update(Roster);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
