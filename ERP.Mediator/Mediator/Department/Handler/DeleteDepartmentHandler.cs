using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Department.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Department.Handler
{
    public class DeleteDepartmentHandler : IRequestHandler<DeleteDepartmentQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteDepartmentHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteDepartmentQuery request, CancellationToken cancellationToken)
        {
            var department = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            department.IsDelete = true;
            department.IsActive = false;
            department.DeleteDate = DateTime.Now;
            department.ModifiedDate = DateTime.Now;
            department.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Department>().Update(department);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
