using System;
using System.Threading;
using System.Threading.Tasks;
using global::AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ERP.BusinessModels.BaseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Auth.Command;
using ERP.Mediator.Mediator.Auth.Validator;
using ERP.Repositories.UnitOfWork;
using System.Linq;
using ERP.Entities.Models;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class AddRoleHandler : BaseHandler, IRequestHandler<AddRoleCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly RegisterValidator validatior;
        private readonly UserManager<AspNetUsersModel> userManager;
        private readonly SessionProvider sessionProvider;

        public AddRoleHandler(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, RegisterValidator validatior, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.validatior = validatior;
            this.userManager = userManager;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            bool isNewRole = string.IsNullOrEmpty(request.Id);

            if (isNewRole)
            {
                var checkDuplicate = await unitOfWork.Repository<AspNetRoles>()
                    .GetAsync(x => x.Name.ToLower() == request.Name.ToLower());

                if (checkDuplicate.Count() == 0)
                {
                    AspNetRoles newRole = new AspNetRoles
                    {
                        Id = Guid.NewGuid(),
                        Name = request.Name,
                        NormalizedName = request.Name.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    };
                    unitOfWork.Repository<AspNetRoles>().Add(newRole);
                    await unitOfWork.SaveChangesAsync();
                    return 200;
                }
                else
                {
                    return 409;
                }
            }
            else
            {
                var role = await unitOfWork.Repository<AspNetRoles>()
                    .GetFirstAsNoTrackingAsync(x => x.Id == new Guid(request.Id));

                var checkDuplicate = await unitOfWork.Repository<AspNetRoles>()
                    .GetAsync(x => x.Name.ToLower() == request.Name.ToLower() && x.Id != new Guid(request.Id));

                if (checkDuplicate.Count() == 0)
                {
                    if (role != null)
                    {
                        role.Name = request.Name;
                        unitOfWork.Repository<AspNetRoles>().Update(role);
                        await unitOfWork.SaveChangesAsync();
                        return 200;
                    }
                    else
                    {
                        return 404;
                    }
                }
                else
                {
                    return 409;
                }
            }
        }


    }
}