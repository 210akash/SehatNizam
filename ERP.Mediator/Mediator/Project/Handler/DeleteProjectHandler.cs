using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Project.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Project.Handler
{
    public class DeleteProjectHandler : IRequestHandler<DeleteProjectQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteProjectHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteProjectQuery request, CancellationToken cancellationToken)
        {
            var Project = await unitOfWork.Repository<Entities.Models.Project>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            Project.IsDelete = true;
            Project.IsActive = false;
            Project.DeleteDate = DateTime.Now;
            Project.ModifiedDate = DateTime.Now;
            Project.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Project>().Update(Project);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
