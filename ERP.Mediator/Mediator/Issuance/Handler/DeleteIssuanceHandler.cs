using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Issuance.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Issuance.Handler
{
    public class DeleteIssuanceHandler : IRequestHandler<DeleteIssuanceQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteIssuanceHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteIssuanceQuery request, CancellationToken cancellationToken)
        {
            var Issuance = await unitOfWork.Repository<Entities.Models.Issuance>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Issuance.IsDelete = true;
            Issuance.IsActive = false;
            Issuance.DeleteDate = DateTime.Now;
            Issuance.ModifiedDate = DateTime.Now;
            Issuance.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Issuance>().Update(Issuance);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
