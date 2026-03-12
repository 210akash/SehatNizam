using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Priority.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Priority.Handler
{
    public class DeletePriorityHandler : IRequestHandler<DeletePriorityQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeletePriorityHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeletePriorityQuery request, CancellationToken cancellationToken)
        {
            var Priority = await unitOfWork.Repository<Entities.Models.Priority>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Priority.IsDelete = true;
            Priority.IsActive = false;
            Priority.DeleteDate = DateTime.Now;
            Priority.ModifiedDate = DateTime.Now;
            Priority.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Priority>().Update(Priority);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
