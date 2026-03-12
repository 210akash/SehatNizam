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
using ERP.Mediator.Mediator.Auth.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class CheckPasswordHandler : BaseHandler, IRequestHandler<LoginCommand, TokenVM>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AspNetUsersModel> userManager;
        private readonly IConfiguration config;
        private readonly IEmailService emailService;
        private readonly ISmsService smsService;
        private readonly IHttpContextAccessor httpContextAccessor;
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckPasswordHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="sessionProvider">The session provider.</param>
        public CheckPasswordHandler(IHttpContextAccessor httpContextAccessor,IMapper mapper, IEmailService emailService, ISmsService smsService, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, IConfiguration config, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
            this.smsService = smsService;
            this.userManager = userManager;
            this.config = config;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<TokenVM> Handle(LoginCommand request, CancellationToken cancellationToken)
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

            var isPasswordtrue = request.IsPasswordHash ? user.PasswordHash == request.Password : await userManager.CheckPasswordAsync(user, request.Password);
            if (isPasswordtrue)
            {
                if (user.TwoFactorEnabled)
                {
                    tokenModel.IsTwoFactorEnabled = true;
                    tokenModel.PhoneNumber = user.PhoneNumberConfirmed ? user.PhoneNumber : "";
                    tokenModel.IsLoginSuccess = true;
                    return tokenModel;
                }
                var userRole = await unitOfWork.Repository<AspNetUserRoles>().FindAllAsync(o => o.UserId == user.Id);

                //if (user.IsAvailableForWeb == false)
                //{
                //    tokenModel.IsLoginSuccess = false;
                //    tokenModel.Error = "Access Denied. Please enable your access and try again.";
                //    return tokenModel;
                //}

                if (!string.IsNullOrEmpty(user.TimeZone))
                {

                    DateTimeOffset localServerTime = DateTimeOffset.Now;

                }

                if (user.DepartmentId != null)
                {
                    user.Department = await unitOfWork.Repository<Entities.Models.Department>().GetFirstAsNoTrackingAsync(o => o.Id == user.DepartmentId, null, null, "Company");
                    tokenModel.Department = user.Department;
                }

                if (user.StoreId != null)
                {
                    user.Store = await unitOfWork.Repository<Entities.Models.Store>().GetFirstAsNoTrackingAsync(o => o.Id == user.StoreId, null, null, null);
                    tokenModel.Store = user.Store;
                }

                user.UserProject = unitOfWork.Repository<UserProject>().Get(o => o.UserId == user.Id && o.IsActive, null, null, "Project", null, null).ToList();
                tokenModel.UserProject = user.UserProject;

                user.IsRemember = request.IsRemember;
                tokenModel.Email = user.Email;
                tokenModel.IsLoginSuccess = true;
                tokenModel.FirstName = user.FirstName;
                tokenModel.LastName = user.LastName;
                tokenModel.PhoneNumber = user.PhoneNumber;
                tokenModel.UserId = user.Id;
                tokenModel.ProfileBlobURl = user.ProfileBlobUrl;
                tokenModel.TimeZone = user.TimeZone;
                tokenModel.StoreId = user.StoreId;
                tokenModel.UserProject = user.UserProject;
                tokenModel.SelectedWarehouseId = user.UserProject.Count > 0 ? user.UserProject.Where(x => x.IsActive).FirstOrDefault().ProjectId : null;

                var data = await this.GenerateSecurityTokenAsync(user);

                tokenModel.Token = data.Data;

                if (tokenModel.Token == "1101")
                {
                    tokenModel.IsLoginSuccess = false;
                    tokenModel.Error = "No Territoy/Distributor assign with this user";
                    return tokenModel;
                }

                tokenModel.Code = user.Code;
                tokenModel.IsDeviceWizardComplete = user.IsDeviceWizardComplete;

                tokenModel.DealershipId = data.DealershipId;
                tokenModel.Dealership = data.Dealership;
                tokenModel.TerritoryId = data.TerritoryId;
                tokenModel.Territory = data.Territory;
                tokenModel.ZoneId = data.ZoneId;
                tokenModel.Zone = data.Zone;
                tokenModel.RetailUserShopId = data.RetailUserShopId;

                foreach (var item in userRole)
                {
                    tokenModel.Role = tokenModel.Role + "," + item.Role.Name;
                }
            }
            else
            {
                tokenModel.IsLoginSuccess = false;
                tokenModel.Error = Constants.InvalidUsrOrPwd;
            }

            return tokenModel;
        }

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

        private async Task<TokenReturnVM> GenerateSecurityTokenAsync(AspNetUsersModel model)
        {
            var roles = await this.GetRolesByUserIdAsync(model.Id);

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(config["JwtSecurityToken:key"]);

            var userRole = await this.unitOfWork.Repository<AspNetUserRoles>().FindAllAsync(o => o.UserId == model.Id);

            long DealershipId = 0;
            long TerritoryId = 0;
            long? ZoneId = 0;
            long? RetailUserShopId = 0;

            TokenReturnVM vm = new TokenReturnVM();

            if (model.Department?.Name == "Sale")
            {
                var check = roles.Any(x => x.AccessCheck == 1);

                if (check)
                {
                    var userTerritory = await this.unitOfWork.Repository<ERP.Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(o => o.UserId == model.Id && o.IsActive && !o.IsDelete, null, null, "Zone,Territory,Territory.Dealership,Territory.Area,Territory.Area.Zone,Territory.Area.Zone.Region");
                    if (userTerritory == null)
                    {
                        vm.Data = "1101";
                        return vm;
                    }
                    else
                    {
                        var _userTerritory = await this.unitOfWork.Repository<ERP.Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(o => o.UserId == model.Id && o.IsActive && !o.IsDelete, null, null, "Zone,Territory,Territory.Dealership");
                        var userTerritoryMap = mapper.Map<GetUserTerritory>(_userTerritory);

                        vm.DealershipId = userTerritoryMap.Territory.Dealership.Where(x => x.IsActive).FirstOrDefault().Id;
                        var entity = await unitOfWork.Repository<ERP.Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(y => y.Id == vm.DealershipId, null, null, "Territory,Territory.Area,Territory.Area.Zone,Territory.Area.Zone.Region");
                        var GetDealership = mapper.Map<GetDealership>(entity);
                        vm.Dealership = GetDealership;

                        if (userTerritoryMap != null)
                        {
                            vm.ZoneId = userTerritoryMap.ZoneId;
                            vm.Zone = userTerritoryMap.Zone;
                            vm.RetailUserShopId = userTerritoryMap.ShopId;

                            if (userTerritoryMap.TerritoryId != 0 || userTerritoryMap.TerritoryId != null)
                            {
                                vm.TerritoryId = userTerritoryMap.TerritoryId;
                                vm.Territory = userTerritoryMap.Territory;
                            }
                        }

                        TerritoryId = userTerritoryMap.TerritoryId.Value;
                        ZoneId = userTerritoryMap.ZoneId;
                        DealershipId = userTerritoryMap.Territory.Dealership.Where(x => x.IsActive).FirstOrDefault().Id;
                        RetailUserShopId = userTerritoryMap.ShopId;
                    }
                }
                else
                {
                    if (model.DealershipId != null && userRole.Any(x => x.Role.Name == "Distributor"))
                    {
                        vm.DealershipId = model.DealershipId;
                        var entity = await unitOfWork.Repository<ERP.Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(y => y.Id == model.DealershipId, null, null, "Territory,Territory.Area,Territory.Area.Zone,Territory.Area.Zone.Region");
                        var GetDealership = mapper.Map<GetDealership>(entity);
                        vm.Dealership = GetDealership;

                        if (GetDealership.TerritoryId != 0)
                        {
                            vm.TerritoryId = GetDealership.TerritoryId;
                            vm.Territory = GetDealership.Territory;
                            if (GetDealership.Territory.Area.ZoneId != 0)
                            {
                                vm.ZoneId = GetDealership.Territory.Area.ZoneId;
                                vm.Zone = GetDealership.Territory.Area.Zone;
                            }
                        }
                    }
                }
            }

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
                    new Claim("DealerShipId", DealershipId.ToString()),
                    new Claim("ZoneId", ZoneId.ToString()),
                    new Claim("TerritoryId", TerritoryId.ToString()),
                    new Claim("StoreId", model.StoreId != null ? model.StoreId.ToString() : "0"),
                    new Claim("SelectedWarehouseId", model.UserProject.Count > 0 ? model.UserProject.Where(x => x.IsActive).FirstOrDefault().ProjectId.ToString() : "0"),
                    new Claim("RetailUserShopId", RetailUserShopId.ToString()),
                    new Claim("DepartmentId", model.DepartmentId != null ? model.DepartmentId.ToString() : "0"),
                    new Claim("CompanyId", model.DepartmentId != null ? model.Department.CompanyId.ToString() : "0"),
                    new Claim("RoleIds", roleIds),
                    new Claim("RoleNames", roleNames)
                }),

                Expires = model.IsRemember ? DateTime.Now.AddDays(30) : DateTime.UtcNow.AddMinutes(double.Parse(config["JwtSecurityToken:DurationInMinutes"])),
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
            vm.Data = tokenHandler.WriteToken(token);
            return vm;
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

        public class TokenReturnVM
        {
            public string Data { get; set; }
            public long? RetailUserShopId { get; set; }

            public long? DealershipId { get; set; }
            public GetDealership Dealership { get; set; }

            public long? TerritoryId { get; set; }
            public GetTerritory Territory { get; set; }

            public long? ZoneId { get; set; }
            public GetZone Zone { get; set; }
        }
    }
}
