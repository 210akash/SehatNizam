namespace ERP.API.Controllers
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using ERP.API.Extensions;
    using ERP.BusinessModels.BaseVM;
    using ERP.BusinessModels.Enums;
    using ERP.BusinessModels.ParameterVM;
    using ERP.BusinessModels.ResponseVM;
    using ERP.Mediator.Mediator.Auth.Command;
    using ERP.Mediator.Mediator.Auth.Query;
    using ERP.Services.Interfaces;
    using ERP.BusinessModels.AttributeExtensions;
    using Microsoft.AspNetCore.SignalR;
    using ERP.Services.Hubs;
    using System.Collections.Generic;
    using ERP.Repositories.UnitOfWork;
    using ERP.Mediator.Mediator.User.Command;
    using ERP.Core.Provider;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IConfiguration config;
        private readonly IMediator mediator;
        private readonly UserManager<AspNetUsersModel> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly StringBuilder trace;
        private readonly IAuthService authService;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILoggerService logger;
        private readonly SessionProvider sessionProvider;

        public AuthController(
            IConfiguration config, IAuthService authService,
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AspNetUsersModel> userManager, SessionProvider sessionProvider, IHubContext<NotificationHub> notificationHubContext, IUnitOfWork unitOfWork, ILoggerService logger)
        {
            this.userManager = userManager;
            this.config = config;
            this.unitOfWork = unitOfWork;
            this.mediator = mediator;
            this.authService = authService;
            this.httpContextAccessor = httpContextAccessor;
            this._notificationHubContext = notificationHubContext;
            this.trace = new StringBuilder();
            this.logger = logger;
            this.sessionProvider = sessionProvider;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var command = new LoginCommand()
            {
                Email = login.Email,
                Password = login.Password,
                IsRemember = login.IsRemember
            };
            if (!ModelState.IsValid)
            {
                var validationErrors = this.GetModelValidationErrors(this.ModelState);
                return this.Result(ResponseStatus.Error, null, validationErrors);
            }

            var jwtToken = await this.mediator.Send(command);
            if (jwtToken.IsLoginSuccess)
            {
                logger.LogInformation("Login To Account", "Login", login.Email, "Auth", "Login", jwtToken.UserId);
                return Ok(jwtToken);
            }

            return this.Result(ResponseStatus.Error, null, jwtToken.Error);
        }

        [HttpPost]
        [Route("LoginMobile")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginMobile([FromBody] LoginMobileModel login)
        {
            var command = new LoginCommand()
            {
                Email = "admin@bwc.com",
                Password = "Admin@123",
                IsRemember = true
            };
            if (!ModelState.IsValid)
            {
                var validationErrors = this.GetModelValidationErrors(this.ModelState);
                return this.Result(ResponseStatus.Error, null, validationErrors);
            }

            var jwtToken = await this.mediator.Send(command);
            if (jwtToken.IsLoginSuccess)
            {

                Meta meta = new()
                {
                    Success = "true",
                    Code = "200"
                };

                Data data = new()
                {
                    access_token = jwtToken.Token
                };

                var token = new TokenMobile()
                {
                    Meta = meta,
                    Data = data
                };

                logger.LogInformation("Login To Account", "Login", login.Cnic, "Auth", "LoginMobile", null);
                return Ok(token);
            }

            return this.Result(ResponseStatus.Error, null, jwtToken.Error);
        }

        [HttpPost]
        [Route("VerifyPassword")]
        public async Task<IActionResult> VerifyPassword(LoginModel login)
        {
            var user = await this.userManager.FindByEmailAsync(login.Email);
            var verifyPassword = await this.userManager.CheckPasswordAsync(user, login.Password);
            return this.Result<bool>(ResponseStatus.OK, verifyPassword);
        }

        [HttpPost]
        [Route("VerifyOTP")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPCommand command)
        {
            var jwtToken = await this.mediator.Send(command);
            if (jwtToken.IsLoginSuccess)
            {
                return this.Result<TokenVM>(ResponseStatus.OK, jwtToken);
            }

            return this.Result(ResponseStatus.Error, null, jwtToken.Error);
        }

        [HttpPost]
        [Route("Send2FAOTP")]
        [AllowAnonymous]
        public async Task<IActionResult> Send2FAOTP([FromBody] Send2FAOTPCommand command)
        {
            try
            {
                var jwtToken = await this.mediator.Send(command);
                return this.Result<bool>(ResponseStatus.OK, jwtToken);
            }
            catch (Exception ex)
            {

                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            if (!ModelState.IsValid)
            {
                var error = this.GetModelValidationErrors(this.ModelState);
                return this.Result(ResponseStatus.Error, false, error);
            }

            var result = await this.mediator.Send(command);
            if (string.IsNullOrEmpty(result.Error) && result.Succeeded)
            {
                var login = new LoginCommand()
                {
                    Email = command.Email,
                    Password = command.Password
                };
                var jwtToken = await this.mediator.Send(login);
                if (jwtToken.IsLoginSuccess)
                {
                    return this.Result<TokenVM>(ResponseStatus.OK, jwtToken);
                }

                logger.LogInformation("Register User", "Register", command.Username, "Auth", "Register", null);
                return this.Result(ResponseStatus.OK, true, Constants.RegConfirmationEmail);
            }

            if (!string.IsNullOrEmpty(result.Error))
            {
                return this.Result(ResponseStatus.Error, false, result.Error);
            }

            var errors = this.GetIdentityResponseErrors(result);
            return this.Result(ResponseStatus.Error, false, errors);
        }

        [HttpPost]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UpdateCommand command)
        {
            try
            {
                await this.mediator.Send(command);
                logger.LogInformation("Update User", "UpdateUser", command.Username, "Auth", "UpdateUser", null);
                return this.Result(ResponseStatus.OK, true, Constants.RegConfirmationEmail);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("ForgetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordQuery query)
        {
            var user = await this.mediator.Send(query);
            return this.Result(ResponseStatus.OK, user.Id, Constants.ResetConfirmationEmailSMS);
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                //await _tokenManager.DeactivateCurrentAsync();

                //return this.Result(ResponseStatus.OK, true, Constants.LogOutSuccessfully);
                var result = await this.mediator.Send(new LogoutCommand());
                if (result.Succeeded)
                {
                    logger.LogInformation("Logout User", "Logout", "Logout", "Auth", "Logout", null);
                    return this.Result(ResponseStatus.OK, true, Constants.LogOutSuccessfully);
                }

                //if (!string.IsNullOrEmpty(result.Error))
                //{
                //    return this.Result(ResponseStatus.Error, false, result.Error);
                //}

                var error = this.GetIdentityResponseErrors(result);
                return this.Result(ResponseStatus.Error, false, error);
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, Constants.GlobalError);
            }
        }

        [HttpPost]
        [Route("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string token, string password)
        {
            var tokenValue = this.authService.Decrypt(token).Split(",");
            var user = await this.userManager.FindByIdAsync(tokenValue[0]);
            if (user == null)
            {
                return this.Result(ResponseStatus.Error, false, Constants.UserNotFound);
            }

            var passwordValidator = new PasswordValidator<AspNetUsersModel>();
            var result = await passwordValidator.ValidateAsync(this.userManager, user, password);
            if (result.Succeeded)
            {
                var resetPasswordResult = await this.userManager.ResetPasswordAsync(user, tokenValue[1], password);
                if (resetPasswordResult.Succeeded)
                {
                    // logout via signalR
                    await _notificationHubContext.Clients.All.SendAsync("resetPassword", new ResetPasswordResponse { UserId = user.Id });
                    var login = new LoginCommand()
                    {
                        Email = user.Email,
                        Password = user.PasswordHash,
                        IsPasswordHash = true
                    };
                    var jwtToken = await this.mediator.Send(login);
                    if (jwtToken.IsLoginSuccess)
                    {
                        return this.Result<TokenVM>(ResponseStatus.OK, jwtToken);
                    }
                    return this.Result(ResponseStatus.OK, true, Constants.PasswordChangeSuccessfull);
                }
                else
                {
                    return this.Result(ResponseStatus.Error, false, Constants.InvalidCode);
                }
            }

            var error = this.GetIdentityResponseErrors(result);
            return this.Result(ResponseStatus.Error, false, error);
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordParameterModel model)
        {
            var user = await this.userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                return this.Result(ResponseStatus.Error, false, Constants.UserNotFound);
            }

            var passwordValidator = new PasswordValidator<AspNetUsersModel>();
            var result = await passwordValidator.ValidateAsync(this.userManager, user, model.NewPassword);
            if (result.Succeeded)
            {
                var changePasswordResult = await this.userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (changePasswordResult.Succeeded)
                {
                    return this.Result(ResponseStatus.OK, changePasswordResult, Constants.ChangePasswordSuccessful);
                }

                var errors = this.GetIdentityResponseErrors(changePasswordResult);
                return this.Result(ResponseStatus.Error, false, errors);
            }

            var error = this.GetIdentityResponseErrors(result);
            return this.Result(ResponseStatus.Error, false, error);
        }

        [HttpGet]
        [Route("ChangeEmail")]
        public async Task<IActionResult> ChangeEmail(string email)
        {
            if (!this.authService.IsValidEmail(email))
            {
                return this.Result(ResponseStatus.Error, false, Constants.InvalidEmail);
            }

            var user = await this.userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return this.Result(ResponseStatus.Error, false, Constants.EmailAlreadyExists);
            }

            var userId = this.GetLoggedInUserId(this.httpContextAccessor);
            await this.authService.ChangeEmail(userId, email);
            return this.Result(ResponseStatus.OK, email, Constants.ResendConfirmationSuccessful);

        }

        [HttpGet]
        [Route("ConfirmChangeEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmChangeEmail(string email, string code)
        {
            if (!this.authService.IsValidEmail(email))
            {
                return this.Result(ResponseStatus.Error, false, Constants.InvalidEmail);
            }
            var userId = this.GetLoggedInUserId(this.httpContextAccessor);
            var responseInfos = await this.authService.ChangeEmailAsync(userId, code, email);
            if (responseInfos.HasErrors())
            {
                return this.Result(ResponseStatus.Error, responseInfos.GetErrors(), Constants.ChangeEmailError);
            }

            return this.Result(ResponseStatus.OK, userId, Constants.ChangeEmailSuccessful);

        }

        [HttpGet]
        [Route("ResendEmailConfirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendEmailConfirmation(Guid userId)
        {
            if (userId != Guid.Empty)
            {
                var user = await this.userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return this.Result(ResponseStatus.Error, false, Constants.InvalidUserId);
                }

                if (user.EmailConfirmed)
                {
                    return this.Result(ResponseStatus.Error, false, Constants.EmailAlreadyConfirmed);
                }

                await this.authService.ResendEmailConfirmationCode(user.Id);

                return this.Result(ResponseStatus.OK, true, Constants.ResendConfirmationSuccessful);
            }

            return this.Result(ResponseStatus.Error, null, Constants.InvalidUserId);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("IsValidPhoneNumber")]
        public async Task<IActionResult> IsValidPhoneNumber(string phoneNumber)
        {
            try
            {
                if (!phoneNumber.StartsWith("+"))
                {
                    phoneNumber = "+" + phoneNumber;
                }

                return this.Result(ResponseStatus.OK, this.authService.IsValidPhoneNumber(phoneNumber));

            }
            catch (Exception ex)
            {

                throw new Exception("Invalid Phone Number");
            }
        }

        [HttpGet]
        [Route("AddPhoneNumber")]
        public async Task<IActionResult> AddPhoneNumber(string phoneNumber)
        {
            try
            {
                if (this.authService.IsValidPhoneNumber(phoneNumber) == false)
                {
                    return this.Result(ResponseStatus.Error, false, Constants.InvalidPhoneNumber);
                }
                if (await this.authService.IsPhoneNumberAlreadyExists(phoneNumber))
                {
                    return this.Result(ResponseStatus.Conflict, false, Constants.PhoneNumberAlreadyExists);
                }

                var currentLoggedInUser = this.GetLoggedInUserId(this.httpContextAccessor);
                phoneNumber = phoneNumber.Trim();
                if (!phoneNumber.StartsWith("+"))
                {
                    phoneNumber = "+" + phoneNumber;
                }
                var user = await this.userManager.FindByIdAsync(currentLoggedInUser.ToString());
                if (user == null)
                {
                    return this.Result(ResponseStatus.Error, false, Constants.UserNotFound);
                }

                var code = await this.userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

                if (await this.authService.SendVerificationCodeOnMobileAsync(code, phoneNumber, true, false))
                {
                    return this.Result(ResponseStatus.OK, true, Constants.PhoneConfirmationSmsSent);
                }

                return this.Result(ResponseStatus.Error, false, Constants.AddPhoneNumberError);
            }
            catch (Exception ex)
            {

                return this.Result(ResponseStatus.Error, false, ex.Message);
            }

        }

        [HttpPost]
        [Route("ResendPhoneNumberVerificationCode")]
        public async Task<IActionResult> ResendPhoneNumberVerificationCode(string phoneNo)
        {
            var currentLoggedInUser = this.GetLoggedInUserId(this.httpContextAccessor);
            phoneNo = phoneNo.Trim();
            if (!phoneNo.StartsWith("+"))
            {
                phoneNo = "+" + phoneNo;
            }
            var user = await this.userManager.FindByIdAsync(currentLoggedInUser.ToString());
            if (user == null)
            {
                return this.Result(ResponseStatus.Error, false, Constants.UserNotFound);
            }

            var code = await this.userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNo);

            if (await this.authService.SendVerificationCodeOnMobileAsync(code, phoneNo, true, false))
            {
                return this.Result(ResponseStatus.OK, true, Constants.ResendPhoneConfirmationCodeSuccessfully);
            }

            return this.Result(ResponseStatus.Error, false, Constants.ResendPhoneConfirmationCodeError);
        }

        [HttpGet]
        [Route("ConfirmPhoneNumber")]
        public async Task<IActionResult> ConfirmPhoneNumber(string token, string phoneNumber)
        {
            //if (!this.authService.IsValidPhoneNumber(phoneNumber))
            //{
            //    return this.Result(ResponseStatus.Error, false, Constants.InvalidPhoneNumber);
            //}

            if (await this.authService.IsPhoneNumberAlreadyExists(phoneNumber))
            {
                return this.Result(ResponseStatus.Error, false, Constants.PhoneNumberAlreadyExists);
            }

            phoneNumber = phoneNumber.Trim();
            if (!phoneNumber.StartsWith("+"))
            {
                phoneNumber = "+" + phoneNumber;
            }
            var currentLoggedInUser = this.GetLoggedInUserId(this.httpContextAccessor);

            var user = await this.userManager.FindByIdAsync(currentLoggedInUser.ToString());
            if (user == null)
            {
                return this.Result(ResponseStatus.Error, false, Constants.UserNotFound);
            }

            var result = await this.userManager.ChangePhoneNumberAsync(user, phoneNumber, token);
            if (result.Succeeded)
            {
                return this.Result(ResponseStatus.OK, true, Constants.PhoneNumberConfirmed);
            }

            var error = this.GetIdentityResponseErrors(result);
            return this.Result(ResponseStatus.Error, false, error);
        }

        [HttpPost]
        [Route("CompleteDeviceWizard")]
        public async Task<IActionResult> CompleteDeviceWizard([FromBody] DeviceWizardCommand command)
        {
            var completeDeviceWizard = await this.mediator.Send(command);
            if (completeDeviceWizard)
            {
                return this.Result(ResponseStatus.OK, completeDeviceWizard);
            }

            return this.Result(ResponseStatus.Error, null, Constants.GlobalError);
        }

        [HttpGet]
        [Route("DecryptToken")]
        [AllowAnonymous]
        public async Task<IActionResult> DecryptToken([FromQuery] DecryptTokenQuery query)
        {
            var model = await this.mediator.Send(query);
            if (model != null)
            {
                return this.Result<DecryptTokenModel>(ResponseStatus.OK, model);
            }
            return this.Result(ResponseStatus.Error);
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<List<GetAllUsers>>> GetAll(string UserName)
        {
            try
            {
                return await this.mediator.Send(new GetAllUsersQuery());
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<ActionResult<List<GetRoles>>> GetAllRoles()
        {
            try
            {
                return await mediator.Send(new GetAspNetRolesQuery());
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("ChangeUserPassword")]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    var user = await this.userManager.FindByIdAsync(command.UserId);
                    var AddPassworsd = await this.userManager.RemovePasswordAsync(user);
                    var AddPassword = await this.userManager.AddPasswordAsync(user, command.Password);

                    return this.Result(ResponseStatus.OK, true, null);
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("AddRole")]
        public async Task<IActionResult> Save(AddRoleCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    var result = await this.mediator.Send(command);
                    if (result == 200)
                    {
                        return this.Result(ResponseStatus.OK, "Role Saved!", null);
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, "Name Already Exists!", null);
                    }
                    else
                    {
                        return this.Result(ResponseStatus.Error, "There is some error!", null);
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }


    }

}