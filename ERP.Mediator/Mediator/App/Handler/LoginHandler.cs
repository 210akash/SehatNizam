using AutoMapper;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Extensions;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.App.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ResponseVM.AppVM;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using SixLabors.ImageSharp;
using System.Linq.Expressions;

namespace ERP.Mediator.Mediator.App.Handler
{
    public class LoginHandler : BaseHandler, IRequestHandler<LoginCommand, AppUserVM>
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
        private readonly ISmsService smsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckPasswordHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="sessionProvider">The session provider.</param>
        public LoginHandler(IMapper mapper, IEmailService emailService, ISmsService smsService, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, IConfiguration config, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
            this.smsService = smsService;
            this.userManager = userManager;
            this.config = config;
        }

        public async Task<AppUserVM> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var tokenModel = new AppUserVM();
            var user = await this.userManager.FindByEmailAsync(request.Email);

            var validationError = this.UserValidation(user);
            if (!string.IsNullOrEmpty(validationError))
            {
                tokenModel.IsLoginSuccess = false;
                tokenModel.Error = validationError;
                return tokenModel;
            }

            var isPasswordtrue = await this.userManager.CheckPasswordAsync(user, request.Password);

            if (isPasswordtrue)
            {

                Expression<Func<Entities.Models.AspNetUsers, bool>> predicate = x => x.IsActive == true && x.Id == user.Id;

                List<string> thenInclude = new List<string>();
                thenInclude.Add("AspNetUserRoles.Role");

                Expression<Func<Entities.Models.AspNetUsers, object>>[] includes = {
                x => x.Attachments,
                x => x.AspNetUserRoles
                };

                var lObjUserEntity = unitOfWork.Repository<Entities.Models.AspNetUsers>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, thenInclude, includes);
                var lObjUser = lObjUserEntity.Item1.ToList().FirstOrDefault();

                tokenModel.Email = user.Email;
                tokenModel.Name = user.FirstName + " " + user.LastName;
                tokenModel.PhoneNumber = user.PhoneNumber;
                tokenModel.UserId = user.Id;
                tokenModel.ShiftTimeStart = lObjUser.ShiftTimeStart;
                tokenModel.ShiftTimeEnd = lObjUser.ShiftTimeEnd;
                tokenModel.DealershipId = user.DealershipId;
                tokenModel.DeviceId = lObjUser.DeviceId;
                tokenModel.IsMobileDeviceRegister = lObjUser.IsMobileDeviceRegister;
                tokenModel.IsAvailableForMobile = lObjUser.IsAvailableForMobile;
                tokenModel.IsAvailableForWeb = lObjUser.IsAvailableForWeb;
                tokenModel.IsDistCompForAtten = lObjUser.IsDistCompForAtten;

                if (lObjUser.AspNetUserRoles == null || lObjUser.AspNetUserRoles.Count() == 0)
                {
                    tokenModel.IsLoginSuccess = false;
                    tokenModel.Error = "No Role Assign to this User";
                }
                else
                {
                    tokenModel.RoleId = lObjUser.AspNetUserRoles.FirstOrDefault().RoleId.ToString();
                    tokenModel.RoleName = lObjUser.AspNetUserRoles.FirstOrDefault().Role.Name;
                    tokenModel.RoleDescription = lObjUser.AspNetUserRoles.FirstOrDefault().Role.Description;
                }

                if (lObjUser.Attachments.FirstOrDefault() != null && lObjUser.Attachments.Count() > 0)
                {
                    tokenModel.Image = lObjUser.Attachments.FirstOrDefault().ImageName;
                }
                //var lObjuser = await unitOfWork.Repository<Entities.Models.AspNetUsers>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.Id == user.Id, null, null, "Attachments");
                var lObjUserAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.UserId == user.Id && o.AttendanceDate.Date == DateTime.Now.Date);
                if (lObjUserAttendance != null && lObjUserAttendance.Id > 0)
                {
                    tokenModel.IsMarkAttendance = true;
                    tokenModel.IsPresent = lObjUserAttendance.IsPresent;
                    tokenModel.IsCheckOut = lObjUserAttendance.CheckOut != null ? true : false;
                    tokenModel.AbsentReason = lObjUserAttendance.Reason;
                }

                tokenModel.IsLoginSuccess = true;
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
    }
}
