using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Referrer.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Referrer.Handler
{
    public class DeleteReferrerHandler : IRequestHandler<DeleteReferrerQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteReferrerHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteReferrerQuery request, CancellationToken cancellationToken)
        {
            var Referrer = await unitOfWork.Repository<Entities.Models.Referrer>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Referrer.IsDelete = true;
            Referrer.IsActive = false;
            Referrer.DeleteDate = DateTime.Now;
            Referrer.ModifiedDate = DateTime.Now;
            Referrer.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Referrer>().Update(Referrer);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
