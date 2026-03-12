using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IGPType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IGPType.Handler
{
    public class DeleteIGPTypeHandler : IRequestHandler<DeleteIGPTypeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteIGPTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteIGPTypeQuery request, CancellationToken cancellationToken)
        {
            var IGPType = await unitOfWork.Repository<Entities.Models.IGPType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            IGPType.IsDelete = true;
            IGPType.IsActive = false;
            IGPType.DeleteDate = DateTime.Now;
            IGPType.ModifiedDate = DateTime.Now;
            IGPType.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.IGPType>().Update(IGPType);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
