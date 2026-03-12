using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.Enums;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Role.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Role.Handler
{
    public class DeleteRoleHandler : IRequestHandler<DeleteRoleQuery, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteRoleHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(DeleteRoleQuery request, CancellationToken cancellationToken)
        {
            var role = await unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(y => y.Id == new Guid(request.Id));
            role.IsActive = false;
            role.IsDelete = true;
            role.DeleteDate = DateTime.Now;
            role.ModifiedDate = DateTime.Now;
            role.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<AspNetRoles>().Update(role);
            var check = await unitOfWork.SaveChangesAsync();
            if (check > 0)
            {
                return (long)ResponseStatus.OK;
            }
            else
            {
                return (long)ResponseStatus.Error;
            }
        }
    }
}
