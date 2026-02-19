//-----------------------------------------------------------------------
// <copyright file="LogoutHandler.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------


    using System.Threading;
    using System.Threading.Tasks;
    using global::AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using ERP.BusinessModels.BaseVM;
    using ERP.BusinessModels.Enums;
    using ERP.BusinessModels.ResponseVM;
    using ERP.Core.Provider;
    using ERP.Mediator.Mediator;
    using ERP.Mediator.Mediator.Auth.Command;
    using ERP.Repositories.UnitOfWork;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    /// <summary>
    /// Check Password Handler
    /// </summary>
    /// <seealso cref="ERP.Mediator.Mediator.BaseHandler" />
    /// <seealso cref="MediatR.IRequestHandler{ERP.Mediator.Mediator.Auth.Command.LoginCommand, ERP.BusinessModels.ResponseVM.TokenVM}" />
    public class LogoutHandler : BaseHandler, IRequestHandler<LogoutCommand, IdentityResult>
    {
        /// <summary>
        /// Mapper Declaration
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Unit of work Declaration
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// User Manager
        /// </summary>
        private readonly UserManager<AspNetUsersModel> userManager;

        /// <summary>
        /// Config declare
        /// </summary>
        private readonly IConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="sessionProvider">The session provider.</param>
        public LogoutHandler(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, IConfiguration config, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.config = config;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<IdentityResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var result = new IdentityResponse();
            var user = await this.userManager.FindByIdAsync(this.SessionProvider.Session.LoggedInUserId.ToString());
            if (user == null)
            {
                result.Error = Constants.UserNotFound;
                return result;
            }
            return await this.userManager.RemoveAuthenticationTokenAsync(user, this.config["JwtSecurityToken:Issuer"], Constants.RefreshToken);
        }
    }
}