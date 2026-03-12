using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IndentRequest.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IndentRequest.Handler
{
    public class DeleteIndentRequestHandler : IRequestHandler<DeleteIndentRequestQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteIndentRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteIndentRequestQuery request, CancellationToken cancellationToken)
        {
            // 1️  Grab the IndentRequest *with* its details and keep it tracked
            var IndentRequest = await unitOfWork.Repository<Entities.Models.IndentRequest>().GetFirstAsync(y => y.Id == request.Id, null, null, "IndentRequestDetail");

            if (IndentRequest is null) return false;

            var now = DateTime.UtcNow;               // safer than Now on servers
            var userId = sessionProvider.Session.LoggedInUserId;

            IndentRequest.IsDelete = true;
            IndentRequest.IsActive = false;
            IndentRequest.DeleteDate = now;
            IndentRequest.ModifiedDate = now;
            IndentRequest.ModifiedById = userId;

            // 3️  Push the same flags into every child row
            foreach (var d in IndentRequest.IndentRequestDetail)
            {
                d.IsDelete = true;
                d.IsActive = false;
                d.DeleteDate = now;
                d.ModifiedDate = now;
                d.ModifiedById = userId;
            }

            unitOfWork.Repository<Entities.Models.IndentRequest>().Update(IndentRequest);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
