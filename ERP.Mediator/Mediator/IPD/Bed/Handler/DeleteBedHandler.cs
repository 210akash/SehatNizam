using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.Bed.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.Bed.Handler
{
    public class DeleteBedHandler : IRequestHandler<DeleteBedQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteBedHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteBedQuery request, CancellationToken cancellationToken)
        {
            var Bed = await unitOfWork.Repository<Entities.Models.Bed>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Bed.IsDelete = true;
            Bed.IsActive = false;
            Bed.DeleteDate = DateTime.Now;
            Bed.ModifiedDate = DateTime.Now;
            Bed.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Bed>().Update(Bed);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
