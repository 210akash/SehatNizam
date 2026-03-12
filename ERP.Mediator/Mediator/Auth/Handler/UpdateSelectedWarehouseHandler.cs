using ERP.Core.Provider;
using ERP.Mediator.Mediator.Auth.Query;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.Repositories.UnitOfWork;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ERP.Core.Extensions;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class UpdateSelectedWarehouseHandler : IRequestHandler<UpdateSelectedWarehouseQuery, string>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration config;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SessionProvider sessionProvider;
        /// <summary>
        /// User Manager
        /// </summary>
        private readonly UserManager<AspNetUsersModel> userManager;
        public UpdateSelectedWarehouseHandler(
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IMapper mapper,
            UserManager<AspNetUsersModel> userManager,
            SessionProvider sessionProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.sessionProvider = sessionProvider;
        }

        public async Task<string> Handle(UpdateSelectedWarehouseQuery request, CancellationToken cancellationToken)
        {
            sessionProvider.Session.SelectedWarehouseId = request.Projectid;
            var context = httpContextAccessor.HttpContext;
            var userId = context.User.FindFirstValue("UserId");
            var user = await userManager.FindByIdAsync(userId);
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Load user's projects
            user.UserProject = unitOfWork.Repository<UserProject>()
                .Get(o => o.UserId == user.Id && o.IsActive, null, null, "Project", null, null).ToList();


            // Manually override the selected warehouse
            var selectedProject = user.UserProject.FirstOrDefault(p => p.ProjectId == request.Projectid);
            if (selectedProject == null)
            {
                throw new Exception("Selected warehouse/project not found for user");
            }

            if (user.DepartmentId != null)
            {
                user.Department = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(o => o.Id == user.DepartmentId, null, null, "Company");
            }

            // Reuse token generation logic
            return await GenerateSecurityTokenAsync(user, request.Projectid);
        }

        private async Task<string> GenerateSecurityTokenAsync(AspNetUsersModel model, long Projectid)
        {
            var roles = await this.GetRolesByUserIdAsync(model.Id);

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(config["JwtSecurityToken:key"]);

            var userRole = await this.unitOfWork.Repository<AspNetUserRoles>().FindAllAsync(o => o.UserId == model.Id);
            var roleIds = string.Join(",", roles.Select(r => r.Id.ToString()));
            var roleNames = string.Join(",", roles.Select(r => r.Name.ToString()));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(JwtRegisteredClaimNames.UniqueName, model.Email),
                    new Claim("UserId", model.Id.ToString()),
                    new Claim("BranchId", "1"),
                    new Claim("RoleId", roles[0].Id.ToString()),
                    new Claim("DealerShipId", "0"),
                    new Claim("ZoneId","0"),
                    new Claim("TerritoryId", "0"),
                    new Claim("StoreId", model.StoreId != null ? model.StoreId.ToString() : "0"),
                    new Claim("SelectedWarehouseId", Projectid.ToString()),
                    new Claim("DepartmentId", model.DepartmentId != null ? model.DepartmentId.ToString() : "0"),
                    new Claim("CompanyId", model.DepartmentId != null ? model.Department.CompanyId.ToString() : "0"),
                    new Claim("RoleIds", roleIds),
                    new Claim("RoleNames", roleNames)
                }),

                Expires = DateTime.Now.AddDays(30),
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
              //  tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, Constants.DefaultRole));
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
