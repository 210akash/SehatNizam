//-----------------------------------------------------------------------
// <copyright file="CheckPasswordHandler.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using global::AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Extensions;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator;
using ERP.Mediator.Mediator.Auth.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using System.Linq;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    /// <summary>
    /// Check Password Handler
    /// </summary>
    /// <seealso cref="TransferMaster.Mediator.Mediator.BaseHandler" />
    /// <seealso cref="IRequestHandler{TransferMaster.Mediator.Mediator.Auth.Command.LoginCommand, TransferMaster.BusinessModels.ResponseVM.TokenVM}" />
    public class VerifyOTPHandler : BaseHandler, IRequestHandler<VerifyOTPCommand, TokenVM>
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

        private readonly IEmailService emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckPasswordHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="sessionProvider">The session provider.</param>
        public VerifyOTPHandler(IMapper mapper, IEmailService emailService, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, IConfiguration config, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
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
        public async Task<TokenVM> Handle(VerifyOTPCommand request, CancellationToken cancellationToken)
        {
            var tokenModel = new TokenVM();
            var user = await userManager.FindByEmailAsync(request.Email);

            var validationError = this.UserValidation(user);
            if (!string.IsNullOrEmpty(validationError))
            {
                tokenModel.IsLoginSuccess = false;
                tokenModel.Error = validationError;
                return tokenModel;
            }



            var result = await userManager.VerifyTwoFactorTokenAsync(user, "Email", request.Token);
            if (result)
            {
                var userRole = await unitOfWork.Repository<AspNetUserRoles>().GetFirstAsync(o => o.UserId == user.Id);

                user.IsRemember = request.IsRemember;
                tokenModel.Email = user.Email;
                tokenModel.IsTwoFactorEnabled = user.TwoFactorEnabled;
                tokenModel.IsLoginSuccess = true;
                tokenModel.FirstName = user.FirstName;
                tokenModel.LastName = user.LastName;
                tokenModel.PhoneNumber = user.PhoneNumber;
                tokenModel.UserId = user.Id;
                tokenModel.ProfileBlobURl = user.ProfileBlobUrl;
                tokenModel.TimeZone = user.TimeZone;
                tokenModel.RoleId = userRole.RoleId;
                tokenModel.Token = await this.GenerateSecurityTokenAsync(user);
                tokenModel.IsDeviceWizardComplete = user.IsDeviceWizardComplete;
            }
            else
            {
                tokenModel.IsLoginSuccess = false;
                tokenModel.Error = Constants.AuthenticationCodeNotVerified;
            }

            return tokenModel;
        }

        /// <summary>
        /// Users the validation.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>return error</returns>
        private string UserValidation(AspNetUsersModel user)
        {
            if (user == null)
            {
                return Constants.InvalidUsrOrPwd;
            }

            if (user.LockoutEnabled)
            {
                return Constants.UserLockedOut;
            }

            if (!user.EmailConfirmed)
            {
                return Constants.EmailNotConfirmed;
            }
            if (!user.IsActive)
            {
                return Constants.UserDeactivated;
            }

            return string.Empty;
        }

        /// <summary>
        /// Generates the security token asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>the token string</returns>
        private async Task<string> GenerateSecurityTokenAsync(AspNetUsersModel model)
        {
            var roles = await this.GetRolesByUserIdAsync(model.Id);

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(config["JwtSecurityToken:key"]);
            var roleIds = string.Join(",", roles.Select(r => r.Name.ToString()));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(JwtRegisteredClaimNames.UniqueName, model.Email),
                    new Claim("UserId", model.Id.ToString()),
                    new Claim("BranchId", "1"),
                    new Claim("DepartmentId", model.DepartmentId.ToString()),
                    new Claim("RoleIds", roleIds)

                }),

                Expires = model.IsRemember ? DateTime.UtcNow.AddYears(5) : DateTime.UtcNow.AddMinutes(double.Parse(config["JwtSecurityToken:DurationInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            if (roles != null && roles.Count > 0)
            {
                foreach (var item in roles)
                {
                    tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, item.Name));
                }
            }
            else
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, Constants.DefaultRole));
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<List<AspNetRoles>> GetRolesByUserIdAsync(Guid userId)
        {
            var userRoles = await unitOfWork.Repository<AspNetUserRoles>().FindAllAsync(s => s.UserId == userId);
            var roles = new List<AspNetRoles>();
            if (userRoles.IsAny())
            {
                foreach (var item in userRoles)
                {
                    var role = await unitOfWork.Repository<AspNetRoles>().FindAsync(x => x.Id == item.RoleId);
                    if (role != null)
                    {
                        roles.Add(role);
                    }
                }
            }

            return roles;
        }
    }
}