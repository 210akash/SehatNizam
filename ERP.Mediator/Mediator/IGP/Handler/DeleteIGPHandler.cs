using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IGP.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IGP.Handler
{
    public class DeleteIGPHandler : IRequestHandler<DeleteIGPQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteIGPHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteIGPQuery request, CancellationToken cancellationToken)
        {
            // 1️  Grab the IGP *with* its details and keep it tracked
            var IGP = await unitOfWork.Repository<Entities.Models.IGP>().GetFirstAsync(y => y.Id == request.Id, null, null, "IGPDetails");

            if (IGP is null) return false;

            var now = DateTime.UtcNow;               // safer than Now on servers
            var userId = sessionProvider.Session.LoggedInUserId;

            IGP.IsDelete = true;
            IGP.IsActive = false;
            IGP.DeleteDate = now;
            IGP.ModifiedDate = now;
            IGP.ModifiedById = userId;

            // 3️  Push the same flags into every child row
            foreach (var d in IGP.IGPDetails)
            {
                d.IsDelete = true;
                d.IsActive = false;
                d.DeleteDate = now;
                d.ModifiedDate = now;
                d.ModifiedById = userId;
            }

            unitOfWork.Repository<Entities.Models.IGP>().Update(IGP);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
