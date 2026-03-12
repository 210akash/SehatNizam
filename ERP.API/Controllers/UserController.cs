//-----------------------------------------------------------------------
// <copyright file="AssetController.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using ERP.API.Extensions;
    using ERP.BusinessModels.BaseVM;
    using ERP.BusinessModels.Enums;
    using ERP.BusinessModels.ResponseVM;
    using ERP.Mediator.Mediator.Auth.Command;
    using ERP.Mediator.Mediator.User.Command;
    using ERP.Mediator.Mediator.User.Query;

    /// <summary>
    /// The AUTH API controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : BaseController
    {
        /// <summary>
        /// Config declare
        /// </summary>
        private readonly IConfiguration config;

        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator mediator;

        /// <summary>
        /// User Manager
        /// </summary>
        private readonly UserManager<AspNetUsersModel> userManager;

        /// <summary>
        /// The HTTP context accessor
        /// </summary>
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// The trace
        /// </summary>
        private readonly StringBuilder trace;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="mediator">The mediator.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="userManager">The user manager.</param>
        public UserController(
            IConfiguration config,
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AspNetUsersModel> userManager)
        {
            this.userManager = userManager;
            this.config = config;
            this.mediator = mediator;
            this.httpContextAccessor = httpContextAccessor;
            this.trace = new StringBuilder();
        }

        [HttpGet]
        public async Task<IActionResult> GetOperatorsCombo([FromQuery] GetOperatorsComboQuery query)
        {
            var model = await this.mediator.Send(query);
            return this.Result<List<AspNetUsersModel>>(ResponseStatus.OK, model);
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
        {
            var model = await this.mediator.Send(query);
            return this.Result<List<GetUserResponse>>(ResponseStatus.OK, model);
        }
        [HttpGet]
        public async Task<IActionResult> GetGeneralUsersCombo([FromQuery] GetGeneralUsersComboQuery query)
        {
            var model = await this.mediator.Send(query);
            return this.Result<List<GetGeneralUserResponse>>(ResponseStatus.OK, model);
        }
        [HttpPost]
        public async Task<IActionResult> DeactivateUser([FromBody] DeactivateUserCommand command)
        {
            var model = await this.mediator.Send(command);
            return this.Result<bool>(ResponseStatus.OK, model);
        }
        [HttpPost]
        public async Task<IActionResult> ActivateUser([FromBody] ActivateUserCommand command)
        {
            var model = await this.mediator.Send(command);
            return this.Result<bool>(ResponseStatus.OK, model);
        }
        [HttpGet]
        public async Task<IActionResult> GetTotalUserRoles([FromQuery] GetTotalUserRolesQuery query)
        {
            var model = await this.mediator.Send(query);
            return this.Result<List<GetTotalUserRolesResponse>>(ResponseStatus.OK, model);
        }

        [HttpPost]
        public async Task<IActionResult> InviteUser([FromBody] InviteUsersCommand command)
        {
            var model = await this.mediator.Send(command);
            return this.Result<bool>(ResponseStatus.OK, model);
        }
        [HttpPost]
        public async Task<IActionResult> SendInvitationReminder([FromBody] SendInvitationReminderCommand command)
        {
            var model = await this.mediator.Send(command);
            return this.Result<DateTime>(ResponseStatus.OK, model);
        }
        [HttpPost]
        public async Task<IActionResult> TransferOwnership([FromBody] TransferOwnershipCommand command)
        {
            var model = await this.mediator.Send(command);
            TokenVM jwtToken = null;
            if (model)
            {
                var currentLoggedInUser = this.GetLoggedInUserId(this.httpContextAccessor);
                var user = await this.userManager.FindByIdAsync(currentLoggedInUser.ToString());
                var login = new LoginCommand()
                {
                    Email = user.Email,
                    Password = user.PasswordHash,
                    IsPasswordHash = true
                };
                jwtToken = await this.mediator.Send(login);
            }
            return this.Result<TokenVM>(ResponseStatus.OK, jwtToken);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleCommand command)
        {
            var model = await this.mediator.Send(command);
            return this.Result<bool>(ResponseStatus.OK, model);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterWithEmail([FromBody] RegisterWithEmailCommand command)
        {
            var model = await this.mediator.Send(command);
            return this.Result<bool>(ResponseStatus.OK, model);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ConfirmRegisterEmail([FromBody] ConfirmRegisterEmailCommand command)
        {
            var model = await this.mediator.Send(command);
            return this.Result<ConfirmRegisterEmailResponse>(ResponseStatus.OK, model);
        }

        #region Roles
        [HttpGet]
        public async Task<IActionResult> GetRolesCombo([FromQuery] GetRolesComboQuery query)
        {
            var model = await this.mediator.Send(query);
            return this.Result<List<AspNetRolesModel>>(ResponseStatus.OK, model);
        }
        #endregion
    }
}