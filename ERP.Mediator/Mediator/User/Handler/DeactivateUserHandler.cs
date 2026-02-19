using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ERP.BusinessModels.BaseVM;
using ERP.Core.Provider;
using ERP.Entities.ComplexTypes;
using ERP.Entities.Models;
using ERP.Mediator.Mediator;
using ERP.Mediator.Mediator.Auth.Command;
using ERP.Mediator.Mediator.User.Command;
using ERP.Mediator.Mediator.User.Query;
using ERP.Repositories.UnitOfWork;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.User.Handler
{
    public class DeactivateUserHandler : BaseHandler, IRequestHandler<DeactivateUserCommand, bool>
    {
        /// <summary>
        /// Mapper Declaration
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Unit of work Declaration
        /// </summary>
        private readonly IUnitOfWork unitOfWork;
        private readonly IUnitOfWorkDapper unitOfWorkDapper;

        /// <summary>
        /// User Manager
        /// </summary>
        private readonly UserManager<AspNetUsersModel> userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="sessionProvider">The session provider.</param>
        public DeactivateUserHandler(IMapper mapper, IUnitOfWork unitOfWork, IUnitOfWorkDapper unitOfWorkDapper, UserManager<AspNetUsersModel> userManager, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.unitOfWorkDapper = unitOfWorkDapper;
        }

        public async Task<bool> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            string[] users = request.UserId.Split(',');
            foreach(var us in users)
            {
                var user = await this.unitOfWork.Repository<AspNetUsers>().GetFirstAsync(u => u.Id == Guid.Parse(us));
                user.IsActive = false;
                this.unitOfWork.Repository<AspNetUsers>().Update(user);
            }
            await this.unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
