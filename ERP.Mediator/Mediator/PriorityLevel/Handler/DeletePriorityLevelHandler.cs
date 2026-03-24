using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.PriorityLevel.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.PriorityLevel.Handler
{
    public class DeletePriorityLevelHandler : IRequestHandler<DeletePriorityLevelQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeletePriorityLevelHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeletePriorityLevelQuery request, CancellationToken cancellationToken)
        {
            var PriorityLevel = await unitOfWork.Repository<Entities.Models.PriorityLevel>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            PriorityLevel.IsDelete = true;
            PriorityLevel.IsActive = false;
            PriorityLevel.DeleteDate = DateTime.Now;
            PriorityLevel.ModifiedDate = DateTime.Now;
            PriorityLevel.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.PriorityLevel>().Update(PriorityLevel);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
