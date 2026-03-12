using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Role.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Role.Handler
{
    public class SaveRoleHandler : IRequestHandler<SaveRoleCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveRoleHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveRoleCommand, long>.Handle(SaveRoleCommand request, CancellationToken cancellationToken)
        {
            var exisitingName = await unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name.ToLower() == request.Name.ToLower() && (request.Id != "" ? x.Id != new Guid(request.Id) : true));
            if(exisitingName == null)
            {
                if (request.Id == "")
                {
                    AspNetRoles role = new AspNetRoles();
                    role.Id = new Guid();
                    role.Name = request.Name;
                    role.NormalizedName = request.Name;
                    role.Description = request.Description;
                    role.AccessCheck = request.AccessCheck;
                    role.IsActive = true;
                    role.IsDelete = false;
                    role.CreatedDate = DateTime.Now;
                    role.CreatedById = sessionProvider.Session.LoggedInUserId;
                    unitOfWork.Repository<AspNetRoles>().Add(role);
                }
                else
                {
                    var existingRole = await unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Id == new Guid(request.Id));
                    existingRole.Name = request.Name;
                    existingRole.NormalizedName = request.Name;
                    existingRole.Description = request.Description;
                    existingRole.AccessCheck = request.AccessCheck;
                    existingRole.ModifiedDate = DateTime.Now;
                    existingRole.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    unitOfWork.Repository<AspNetRoles>().Update(existingRole);
                }
                SaveChanges();
                return 200;
            }
            else
            {
                return 409;
            }
        }
    }
}