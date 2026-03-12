using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.RejectReason.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.RejectReason.Handler
{
    public class DeleteRejectReasonHandler : IRequestHandler<DeleteRejectReasonQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteRejectReasonHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteRejectReasonQuery request, CancellationToken cancellationToken)
        {
            var RejectReason = await unitOfWork.Repository<Entities.Models.RejectReason>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            RejectReason.IsDelete = true;
            RejectReason.IsActive = false;
            RejectReason.DeleteDate = DateTime.Now;
            RejectReason.ModifiedDate = DateTime.Now;
            RejectReason.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.RejectReason>().Update(RejectReason);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
