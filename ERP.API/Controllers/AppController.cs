using AutoMapper;
using ERP.API.Extensions;
using ERP.BusinessModels.Enums;
using ERP.BusinessModels.ParameterVM;
using ERP.BusinessModels.ResponseVM;
using ERP.BusinessModels.ResponseVM.AppVM;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.App.Command;
using ERP.Mediator.Mediator.App.Query;
using ERP.Mediator.Mediator.Appointment.Command;
using ERP.Mediator.Mediator.Appointment.Query;
using ERP.Mediator.Mediator.Shop.Query;
using ERP.Mediator.Mediator.ShopDispatch.Command;
using ERP.Mediator.Mediator.ShopDispatch.Query;
using ERP.Mediator.Mediator.ShopOrder.Command;
using ERP.Mediator.Mediator.ShopOrder.Query;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;

namespace ERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly IMediator mediator;
        private string SecurityToken = "6XesrAM2Nu";
        //Khilafat Company
        private int lIntKhilafatCompanyId = 2;
        private readonly IUnitOfWork unitOfWork;
        private readonly IBlobService blobService;
        private readonly IMapper mapper;
        private readonly IConfiguration _configuration;
        private readonly string Localcontainer;

        public AppController(IUnitOfWork unitOfWork, IMediator mediator, IBlobService blobService, IMapper mapper, IConfiguration configuration)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.blobService = blobService;
            this.mapper = mapper;
            _configuration = configuration;
            Localcontainer = _configuration["LocalBlob:BlobContainerName"];
        }

        #region Login and Administrator
        [HttpGet]
        [Route("IsValidToken")]
        private bool IsValidToken(string requestToken)
        {
            if (string.IsNullOrWhiteSpace(requestToken) || requestToken != SecurityToken)
            {
                return false;
            }
            else if (requestToken == SecurityToken)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpPost]
        [Route("MarkLoginLogoutState")]
        public async Task<ActionResult<string>> MarkLoginLogoutState(MarkLoginLogoutState command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }

                    var lObjUserEntityUpdate = await unitOfWork.Repository<AspNetUsers>().GetFirstAsync(x => x.IsActive == true && x.DeviceId == command.DeviceId);
                    if (lObjUserEntityUpdate != null)
                    {
                        if (command.IsLogin == true)
                        {
                            lObjUserEntityUpdate.IsLogedIn = true;
                        }
                        else
                        {
                            lObjUserEntityUpdate.IsLogedIn = false;
                        }
                        lObjUserEntityUpdate.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<AspNetUsers>().Update(lObjUserEntityUpdate);
                        unitOfWork.SaveChanges();
                        return this.Result(ResponseStatus.OK, null, "Login State Change");
                    }
                    else
                    {
                        return this.Result(ResponseStatus.Error, null, "User not Exists");
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<string>> Login(LoginCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }


                    var lObjLoginCommand = new LoginCommand()
                    {
                        Email = command.Email,
                        Password = command.Password
                    };

                    var jwtToken = await this.mediator.Send(command);

                    if (jwtToken.IsLoginSuccess)
                    {
                        //  Check if any other user in login with same device
                        //var lObjUserEntityUpdateDevice = await unitOfWork.Repository<AspNetUsers>().FindAllAsync(x => x.IsActive == true && x.DeviceId == command.DeviceId);
                        //foreach (var item in lObjUserEntityUpdateDevice)
                        //{
                        //    item.IsLogedIn = false;
                        //    item.IsMobileDeviceRegister = false;
                        //    item.DeviceId = null;
                        //    unitOfWork.Repository<AspNetUsers>().Update(item);
                        //    unitOfWork.SaveChanges();
                        //}

                        //  Check if any other user in login with same device
                        var lObjUserEntityUpdate = await unitOfWork.Repository<AspNetUsers>().GetFirstAsync(x => x.IsActive == true && x.Id == jwtToken.UserId);
                        if (lObjUserEntityUpdate.DeviceId != null && lObjUserEntityUpdate.DeviceId != command.DeviceId)
                        {
                            lObjUserEntityUpdate.IsLogedIn = false;
                            lObjUserEntityUpdate.IsMobileDeviceRegister = false;
                            jwtToken.IsMobileDeviceRegister = false;
                            jwtToken.IsNewDevice = true;
                        }
                        lObjUserEntityUpdate.DeviceId = command.DeviceId;
                        lObjUserEntityUpdate.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<AspNetUsers>().Update(lObjUserEntityUpdate);
                        unitOfWork.SaveChanges();

                        if (lObjUserEntityUpdate.IsMobileDeviceRegister == true)
                        {
                            var lObjSAmeDeviceUserEntityUpdate = from u in unitOfWork.Repository<AspNetUsers>().GetAll()
                                                                 where u.DeviceId == command.DeviceId && u.Id != lObjUserEntityUpdate.Id
                                                                 select u;

                            foreach (var user in lObjSAmeDeviceUserEntityUpdate)
                            {
                                user.DeviceId = null;
                                user.IsLogedIn = false;
                                user.IsMobileDeviceRegister = null;
                                unitOfWork.Repository<AspNetUsers>().Update(user);
                            }
                            unitOfWork.SaveChanges();

                        }
                        jwtToken.IsMobileDeviceRegister = jwtToken.IsMobileDeviceRegister == null ? false : jwtToken.IsMobileDeviceRegister;
                        jwtToken.IsAvailableForMobile = jwtToken.IsAvailableForMobile == null ? false : jwtToken.IsAvailableForMobile;
                        jwtToken.IsDistCompForAtten = jwtToken.IsDistCompForAtten == null ? false : jwtToken.IsDistCompForAtten;
                        jwtToken.Image = jwtToken.Image !=  null ? GetImageAsBase64(Path.Combine(Localcontainer, jwtToken.Image.TrimStart('/'))) : "";
                        //In Users k lye device registration nahi zaroori, Ye testing user hain or First 1 for google 

                        var allowedEmails = _configuration["AllowedEmails:MobileToken"]?
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(e => e.Trim())
                            .ToHashSet(StringComparer.OrdinalIgnoreCase)
                            ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                        if (allowedEmails.Contains(jwtToken.Email.ToLower()))
                        {
                            jwtToken.IsMobileDeviceRegister = true;
                        }
                        return this.Result(ResponseStatus.OK, jwtToken, "Login Successfully");
                    }

                    return this.Result(ResponseStatus.Error, null, jwtToken.Error);
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        private string GetJsonPinLocation(decimal lat, decimal lng)
        {
            var location = new
            {
                lat = (double)lat, // Convert decimal to double for JSON
                lng = (double)lng  // Convert decimal to double for JSON
            };

            return JsonSerializer.Serialize(location);
        }

        [HttpPost]
        [Route("MarkAttendance")]
        public async Task<ActionResult<string>> MarkAttendance(MarkAttendanceCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (command.IsPresent == false && string.IsNullOrWhiteSpace(command.Reason))
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Reason Is Compulsory in Leave Case");
                    }
                    if (command.lat == null || command.lat == 0 || command.lng == null || command.lng == 0)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Location not properly sent");
                    }

                    var lObjUserAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.UserId == new Guid(command.UserId) && o.AttendanceDate.Date == DateTime.Now.Date);
                    if (lObjUserAttendance != null && lObjUserAttendance.Id > 0)
                    {
                        return this.Result(ResponseStatus.Error, null, "Attendance Already Mark");
                    }
                    var lObjUsers = await unitOfWork.Repository<Entities.Models.AspNetUsers>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.Id == new Guid(command.UserId));

                    if (!string.IsNullOrWhiteSpace(lObjUsers.WeeklyOff))
                    {
                        string today = DateTime.Now.DayOfWeek.ToString();
                        bool IsTodayOff = lObjUsers.WeeklyOff.Split(',').Contains(today);
                        if (IsTodayOff)
                        {
                            return this.Result(ResponseStatus.WeeklyOff, null, "Today is your weekly Off!");
                        }
                    }

                    UserAttendance _UserAttendance = new UserAttendance();

                    _UserAttendance.CreatedById = new Guid(command.UserId);
                    _UserAttendance.CreatedDate = DateTime.Now;
                    _UserAttendance.AttendanceDate = DateTime.Now;
                    _UserAttendance.IsPresent = command.IsPresent;
                    _UserAttendance.TimeIn = DateTime.Now;
                    _UserAttendance.DeviceType = (int)BusinessModels.Enums.DeviceType.Mobile;
                    _UserAttendance.AttendanceType = (int)BusinessModels.Enums.AttendanceType.Present;
                    _UserAttendance.PinLocation = GetJsonPinLocation(command.lat, command.lng);
                    _UserAttendance.DealershipId = command.DealershipId;

                    if (_UserAttendance.IsPresent == false)
                    {
                        _UserAttendance.Reason = command.Reason;
                    }
                    _UserAttendance.UserId = new Guid(command.UserId);

                    unitOfWork.Repository<Entities.Models.UserAttendance>().Add(_UserAttendance);
                    unitOfWork.SaveChanges();
                    if (command.IsPresent == false)
                    {
                        return this.Result(ResponseStatus.OK, null, "Leave Mark Successfully");
                    }
                    else
                    {
                        return this.Result(ResponseStatus.OK, null, "Attendance Mark Successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("MarkAttendanceCheckOut")]
        public async Task<ActionResult<string>> MarkAttendanceCheckOut(MarkAttendanceCheckOutCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (command.lat == null || command.lat == 0 || command.lng == null || command.lng == 0)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Location not properly sent");
                    }

                    var lObjUserAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.UserId == new Guid(command.UserId) && o.AttendanceDate.Date == DateTime.Now.Date);

                    if (lObjUserAttendance == null)
                    {
                        return this.Result(ResponseStatus.Conflict, null, "Attendance Not Marked");
                    }
                    else if (lObjUserAttendance.IsPresent == false)
                    {
                        return this.Result(ResponseStatus.Conflict, null, "User on Leave");
                    }
                    if (lObjUserAttendance.CheckOut != null)
                    {
                        return this.Result(ResponseStatus.Conflict, null, "Checkout already marked");
                    }
                    //Role RSM, ZSM AND ASM
                    var roleIds = new List<Guid>
                                    {
                                        new Guid("E078294A-6FE8-44E1-5D66-08DD12BEBAD2"),
                                        new Guid("C1D2B6FE-E34B-45CC-A7F6-08DCD318EE1A"),
                                        new Guid("AF7275E0-7DD2-4419-5D67-08DD12BEBAD2")
                                    };

                    var result = (
                        from usr in unitOfWork.Repository<AspNetUsers>().GetAll()
                        join usrr in unitOfWork.Repository<AspNetUserRoles>().GetAll()
                            on usr.Id equals usrr.UserId
                        join rol in unitOfWork.Repository<AspNetRoles>().GetAll()
                            on usrr.RoleId equals rol.Id
                        where usr.Id == new Guid(command.UserId)
                              && roleIds.Contains(rol.Id)
                        select new
                        {
                            UserId = usr.Id,
                            RoleName = rol.Name
                        }
                    ).ToList();

                    //Exclude ZSM and ASM 8 Hours check 
                    if (result.Count() == 0)
                    {
                        TimeSpan timeDifference = DateTime.Now - lObjUserAttendance.AttendanceDate;
                        double totalHours = timeDifference.TotalHours;
                        if (totalHours < 8)
                        {
                            //return this.Result(ResponseStatus.Conflict, null, "Working hours: " + totalHours.ToString() + ", It Should be Atleast 8 Hours");
                            return this.Result(ResponseStatus.Conflict, null, "Your Working Hours are not completed yet. Make Sure to complete your working hours");
                        }
                    }

                    lObjUserAttendance.CheckOut = DateTime.Now;
                    lObjUserAttendance.TimeOut = DateTime.Now;
                    if (lObjUserAttendance.TimeIn.HasValue && lObjUserAttendance.TimeOut.HasValue)
                    {
                        TimeSpan timeDifferenceBtw = lObjUserAttendance.TimeOut.Value - lObjUserAttendance.TimeIn.Value;
                        // Optional: validate the duration is positive
                        if (timeDifferenceBtw.TotalMinutes > 0)
                        {
                            lObjUserAttendance.WorkingHours = (decimal)timeDifferenceBtw.TotalHours;
                        }
                        else
                        {
                            lObjUserAttendance.WorkingHours = null;
                        }
                    }
                    else
                    {
                        lObjUserAttendance.WorkingHours = null;
                    }
                    lObjUserAttendance.CheckOutLocation = GetJsonPinLocation(command.lat, command.lng);

                    unitOfWork.Repository<UserAttendance>().Update(lObjUserAttendance);
                    unitOfWork.SaveChanges();

                    return this.Result(ResponseStatus.OK, null, "Checkout Marked Successfully");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("GetUserAttendanceByUserId")]
        public async Task<ActionResult<string>> GetUserAttendanceByUserId([FromQuery] string userId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var result = (
                              from ut in unitOfWork.Repository<UserAttendance>().GetAll()
                              join usr in unitOfWork.Repository<AspNetUsers>().GetAll()
                                  on ut.UserId equals usr.Id
                              where ut.IsActive == true
                                    && ut.IsDelete == false
                                    && ut.UserId == new Guid(userId)
                              orderby ut.AttendanceDate descending
                              select new
                              {
                                  Email = usr.Email,
                                  Name = usr.FirstName + " " + usr.LastName,
                                  IsPresent = ut.IsPresent,
                                  AttendanceDate = ut.AttendanceDate,
                                  Reason = ut.Reason,
                                  PinLocation = ut.PinLocation,
                                  CheckOut = ut.CheckOut,
                                  CheckOutLocation = ut.CheckOutLocation
                              }
                          );

                    var QueryResult = result.ToList();

                    return this.Result(ResponseStatus.OK, QueryResult, QueryResult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("GetUserDetailsByUserId")]
        public async Task<ActionResult<string>> GetUserDetailsByUserId(GetUserDetailsByUserIdCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }


                    Expression<Func<Entities.Models.AspNetUsers, bool>> predicate = x => x.IsActive == true && x.Id == new Guid(command.UserId);

                    List<string> thenInclude = new List<string>();
                    thenInclude.Add("AspNetUserRoles.Role");
                    thenInclude.Add("EmployeeDesignation");

                    Expression<Func<Entities.Models.AspNetUsers, object>>[] includes = {
                    x => x.Attachments,
                    x => x.AspNetUserRoles
                    };


                    var tokenModel = new AppUserVM();
                    var lObjUserEntity = unitOfWork.Repository<Entities.Models.AspNetUsers>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, thenInclude, includes);

                    if (lObjUserEntity.Item2 == 0)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "User not exist!");
                    }


                    var lObjUser = lObjUserEntity.Item1.ToList().FirstOrDefault();


                    if (lObjUser.DeviceId != command.DeviceId)
                    {
                        tokenModel.IsNewDevice = true;
                    }
                    else
                    {
                        tokenModel.IsNewDevice = false;
                    }

                    tokenModel.Email = lObjUser.Email;
                    tokenModel.Name = lObjUser.FirstName + " " + lObjUser.LastName;
                    tokenModel.PhoneNumber = lObjUser.PhoneNumber;
                    tokenModel.UserId = lObjUser.Id;
                    tokenModel.ShiftTimeStart = lObjUser.ShiftTimeStart;
                    tokenModel.ShiftTimeEnd = lObjUser.ShiftTimeEnd;
                    tokenModel.DeviceId = lObjUser.DeviceId;
                    tokenModel.IsMobileDeviceRegister = lObjUser.IsMobileDeviceRegister;
                    tokenModel.IsAvailableForMobile = lObjUser.IsAvailableForMobile;
                    tokenModel.IsAvailableForWeb = lObjUser.IsAvailableForWeb;
                    tokenModel.IsDistCompForAtten = lObjUser.IsDistCompForAtten;
                    tokenModel.EmployeeDesignation = lObjUser.EmployeeDesignation != null ? lObjUser.EmployeeDesignation.Name : "";

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

                    if (tokenModel.RoleName.ToLower() == "ase" || tokenModel.RoleName.ToLower() == "dsf" || tokenModel.RoleName.ToLower() == "asd")
                    {
                        var UserTerritory = await unitOfWork.Repository<ERP.Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && x.UserId == new Guid(command.UserId), null, null, "Territory,Territory.Dealership");
                        if (UserTerritory != null && UserTerritory.Territory != null && UserTerritory.Territory.Dealership != null && UserTerritory.Territory.Dealership.Count > 0)
                        {
                            tokenModel.DealershipId = UserTerritory.Territory.Dealership.FirstOrDefault().Id;
                            tokenModel.DealershipName = UserTerritory.Territory.Dealership.FirstOrDefault().Name;
                            tokenModel.DealershipLocation = UserTerritory.Territory.Dealership.FirstOrDefault().PinLocation;

                            var distLat = JsonDocument.Parse(tokenModel.DealershipLocation).RootElement.GetProperty("lat").GetDouble();
                            var distLng = JsonDocument.Parse(tokenModel.DealershipLocation).RootElement.GetProperty("lng").GetDouble();
                            var distanceInMeters = 6371000 * Math.Acos(
                                                    Math.Cos(DegreeToRadian(command.lat)) * Math.Cos(DegreeToRadian(distLat)) *
                                                    Math.Cos(DegreeToRadian(distLng) - DegreeToRadian(command.lng)) +
                                                    Math.Sin(DegreeToRadian(command.lat)) * Math.Sin(DegreeToRadian(distLat)));

                            tokenModel.DistanceInMeters = distanceInMeters;

                            // Conditional distance formatting
                            tokenModel.FormattedDistance = distanceInMeters < 1000 ? distanceInMeters.ToString("F0") : (distanceInMeters / 1000.0).ToString("F2");
                            tokenModel.FormattedDistanceUnit = distanceInMeters < 1000 ? "m" : "km";
                        }
                    }

                    if (lObjUser.Attachments.FirstOrDefault() != null && lObjUser.Attachments.Count() > 0)
                    {
                        //tokenModel.Image = lObjUser.Attachments.FirstOrDefault().ImageName;

                        string imageName = lObjUser.Attachments.FirstOrDefault()?.ImageName?.TrimStart('/') ?? "";

                        tokenModel.Image = GetImageAsBase64(Path.Combine(Localcontainer, imageName));
                    }
                    //var lObjuser = await unitOfWork.Repository<Entities.Models.AspNetUsers>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.Id == user.Id, null, null, "Attachments");
                    var lObjUserAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.UserId == new Guid(command.UserId) && o.AttendanceDate.Date == DateTime.Now.Date);
                    if (lObjUserAttendance != null && lObjUserAttendance.Id > 0)
                    {
                        tokenModel.IsMarkAttendance = true;
                        tokenModel.IsPresent = lObjUserAttendance.IsPresent;
                        tokenModel.PresentTime = lObjUserAttendance.AttendanceDate;
                        tokenModel.IsCheckOut = lObjUserAttendance.CheckOut != null ? true : false;
                        tokenModel.CheckOutTime = lObjUserAttendance.CheckOut != null ? lObjUserAttendance.CheckOut : null;
                        tokenModel.AbsentReason = lObjUserAttendance.Reason;
                    }


                    tokenModel.IsLoginSuccess = true;
                    tokenModel.IsMobileDeviceRegister = tokenModel.IsMobileDeviceRegister == null ? false : tokenModel.IsMobileDeviceRegister;
                    tokenModel.IsAvailableForMobile = tokenModel.IsAvailableForMobile == null ? false : tokenModel.IsAvailableForMobile;
                    tokenModel.IsDistCompForAtten = tokenModel.IsDistCompForAtten == null ? false : tokenModel.IsDistCompForAtten;
                    return this.Result(ResponseStatus.OK, tokenModel, null);
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("GetUserDetailsByDeviceId")]
        public async Task<ActionResult<string>> GetUserDetailsByDeviceId(GetUserDetailsByDeviceIdCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }

                    Expression<Func<Entities.Models.AspNetUsers, bool>> predicate = x => x.IsActive == true && x.DeviceId == command.DeviceId;

                    List<string> thenInclude = new List<string>();
                    thenInclude.Add("AspNetUserRoles.Role");
                    thenInclude.Add("EmployeeDesignation");

                    Expression<Func<Entities.Models.AspNetUsers, object>>[] includes = {
                    x => x.Attachments,
                    x => x.AspNetUserRoles
                    };


                    var tokenModel = new AppUserVM();
                    var lObjUserEntity = unitOfWork.Repository<Entities.Models.AspNetUsers>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, thenInclude, includes);



                    if (lObjUserEntity.Item2 == 0)
                    {
                        return this.Result(ResponseStatus.NoUserExists, null, "User not exist!");
                    }


                    var lObjUser = lObjUserEntity.Item1.ToList().FirstOrDefault();


                    if (lObjUser.DeviceId != command.DeviceId)
                    {
                        tokenModel.IsNewDevice = true;
                    }
                    else
                    {
                        tokenModel.IsNewDevice = false;
                    }
                    tokenModel.Email = lObjUser.Email;
                    tokenModel.Name = lObjUser.FirstName + " " + lObjUser.LastName;
                    tokenModel.PhoneNumber = lObjUser.PhoneNumber;
                    tokenModel.UserId = lObjUser.Id;
                    tokenModel.ShiftTimeStart = lObjUser.ShiftTimeStart;
                    tokenModel.ShiftTimeEnd = lObjUser.ShiftTimeEnd;
                    tokenModel.DeviceId = lObjUser.DeviceId;
                    tokenModel.IsMobileDeviceRegister = lObjUser.IsMobileDeviceRegister;
                    tokenModel.IsAvailableForMobile = lObjUser.IsAvailableForMobile;
                    tokenModel.IsAvailableForWeb = lObjUser.IsAvailableForWeb;
                    tokenModel.IsDistCompForAtten = lObjUser.IsDistCompForAtten;
                    tokenModel.IsLogedIn = lObjUser.IsLogedIn;
                    tokenModel.EmployeeDesignation = lObjUser.EmployeeDesignation != null ? lObjUser.EmployeeDesignation.Name : "";

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

                    if (tokenModel.RoleName.ToLower() == "rsm")
                    {
                        //Reginoal Level
                        var UserTerritory = from u in unitOfWork.Repository<UserTerritory>().GetAll()
                                            where u.IsActive == true && u.IsDelete == false && u.UserId == lObjUser.Id
                                            select u;

                        if (UserTerritory.Count() > 0)
                        {
                            tokenModel.lstDealershipDetails = new List<DealershipDetails>();
                            foreach (var lObjuserTerritory in UserTerritory)
                            {
                                if (lObjuserTerritory.RegionId == null || lObjuserTerritory.RegionId == 0)
                                {
                                    continue;
                                }
                                var result = from reg in unitOfWork.Repository<Entities.Models.Region>().GetAll()
                                             join zon in unitOfWork.Repository<Entities.Models.Zone>().GetAll() on reg.Id equals zon.RegionId
                                             join area in unitOfWork.Repository<Entities.Models.Area>().GetAll() on zon.Id equals area.ZoneId
                                             join ter in unitOfWork.Repository<Entities.Models.Territory>().GetAll() on area.Id equals ter.AreaId
                                             join dis in unitOfWork.Repository<Entities.Models.Dealership>().GetAll() on ter.Id equals dis.TerritoryId
                                             where dis.DealershipTypeId == 1 && dis.Id != 7 && reg.IsActive == true && zon.IsActive == true && area.IsActive == true && ter.IsActive == true && dis.IsActive == true
                                                   && reg.Id == lObjuserTerritory.RegionId
                                             select new
                                             {
                                                 dis.Id,
                                                 dis.Name,
                                                 dis.PhoneNo,
                                                 dis.Address,
                                                 dis.PinLocation
                                             };

                                var dealershipList = result.ToList();


                                foreach (var item in dealershipList)
                                {
                                    DealershipDetails lObjDealershipDetails = new DealershipDetails();
                                    lObjDealershipDetails.DealershipId = item.Id;
                                    lObjDealershipDetails.DealershipName = item.Name;
                                    lObjDealershipDetails.PhoneNo = item.PhoneNo;
                                    lObjDealershipDetails.Address = item.Address;
                                    lObjDealershipDetails.DealershipLocation = item.PinLocation;

                                    var distLat = JsonDocument.Parse(item.PinLocation).RootElement.GetProperty("lat").GetDouble();
                                    var distLng = JsonDocument.Parse(item.PinLocation).RootElement.GetProperty("lng").GetDouble();
                                    var distanceInMeters = 6371000 * Math.Acos(
                                                            Math.Cos(DegreeToRadian(command.lat)) * Math.Cos(DegreeToRadian(distLat)) *
                                                            Math.Cos(DegreeToRadian(distLng) - DegreeToRadian(command.lng)) +
                                                            Math.Sin(DegreeToRadian(command.lat)) * Math.Sin(DegreeToRadian(distLat)));



                                    lObjDealershipDetails.DistanceInMeters = distanceInMeters;

                                    // Conditional distance formatting
                                    lObjDealershipDetails.FormattedDistance = distanceInMeters < 1000 ? distanceInMeters.ToString("F0") : (distanceInMeters / 1000.0).ToString("F2");
                                    lObjDealershipDetails.FormattedDistanceUnit = distanceInMeters < 1000 ? "m" : "km";
                                    tokenModel.lstDealershipDetails.Add(lObjDealershipDetails);
                                }

                            }

                            tokenModel.lstDealershipDetails = tokenModel.lstDealershipDetails
                                                                .OrderBy(d => d.DistanceInMeters)  // Order by distance, nearest first
                                                                .ToList();
                        }
                    }
                    else if (tokenModel.RoleName.ToLower() == "zsm")
                    {
                        // Zone Level 
                        var UserTerritory = from u in unitOfWork.Repository<UserTerritory>().GetAll()
                                            where u.IsActive == true && u.IsDelete == false && u.UserId == lObjUser.Id
                                            select u;

                        if (UserTerritory.Count() > 0)
                        {
                            tokenModel.lstDealershipDetails = new List<DealershipDetails>();
                            foreach (var lObjuserTerritory in UserTerritory)
                            {
                                if (lObjuserTerritory.ZoneId == null || lObjuserTerritory.ZoneId == 0)
                                {
                                    continue;
                                }
                                var result = from zon in unitOfWork.Repository<Entities.Models.Zone>().GetAll()
                                             join area in unitOfWork.Repository<Entities.Models.Area>().GetAll() on zon.Id equals area.ZoneId
                                             join ter in unitOfWork.Repository<Entities.Models.Territory>().GetAll() on area.Id equals ter.AreaId
                                             join dis in unitOfWork.Repository<Entities.Models.Dealership>().GetAll() on ter.Id equals dis.TerritoryId
                                             where dis.DealershipTypeId == 1 && zon.IsActive == true && area.IsActive == true && ter.IsActive == true && dis.IsActive == true
                                                   && zon.Id == lObjuserTerritory.ZoneId
                                             select new
                                             {
                                                 dis.Id,
                                                 dis.Name,
                                                 dis.PhoneNo,
                                                 dis.Address,
                                                 dis.PinLocation
                                             };

                                var dealershipList = result.ToList();


                                foreach (var item in dealershipList)
                                {
                                    DealershipDetails lObjDealershipDetails = new DealershipDetails();
                                    lObjDealershipDetails.DealershipId = item.Id;
                                    lObjDealershipDetails.DealershipName = item.Name;
                                    lObjDealershipDetails.PhoneNo = item.PhoneNo;
                                    lObjDealershipDetails.Address = item.Address;
                                    lObjDealershipDetails.DealershipLocation = item.PinLocation;

                                    var distLat = JsonDocument.Parse(item.PinLocation).RootElement.GetProperty("lat").GetDouble();
                                    var distLng = JsonDocument.Parse(item.PinLocation).RootElement.GetProperty("lng").GetDouble();
                                    var distanceInMeters = 6371000 * Math.Acos(
                                                            Math.Cos(DegreeToRadian(command.lat)) * Math.Cos(DegreeToRadian(distLat)) *
                                                            Math.Cos(DegreeToRadian(distLng) - DegreeToRadian(command.lng)) +
                                                            Math.Sin(DegreeToRadian(command.lat)) * Math.Sin(DegreeToRadian(distLat)));



                                    lObjDealershipDetails.DistanceInMeters = distanceInMeters;

                                    // Conditional distance formatting
                                    lObjDealershipDetails.FormattedDistance = distanceInMeters < 1000 ? distanceInMeters.ToString("F0") : (distanceInMeters / 1000.0).ToString("F2");
                                    lObjDealershipDetails.FormattedDistanceUnit = distanceInMeters < 1000 ? "m" : "km";
                                    tokenModel.lstDealershipDetails.Add(lObjDealershipDetails);
                                }

                            }

                            tokenModel.lstDealershipDetails = tokenModel.lstDealershipDetails
                                                                .OrderBy(d => d.DistanceInMeters)  // Order by distance, nearest first
                                                                .ToList();
                        }

                    }
                    else if (tokenModel.RoleName.ToLower() == "asm" || tokenModel.RoleName.ToLower() == "ase" || tokenModel.RoleName.ToLower() == "asd")
                    {
                        //Area Level
                        var UserTerritory = from u in unitOfWork.Repository<UserTerritory>().GetAll()
                                            where u.IsActive == true && u.IsDelete == false && u.UserId == lObjUser.Id
                                            select u;

                        if (UserTerritory.Count() > 0)
                        {
                            tokenModel.lstDealershipDetails = new List<DealershipDetails>();
                            foreach (var lObjuserTerritory in UserTerritory)
                            {
                                if (lObjuserTerritory.AreaId == null || lObjuserTerritory.AreaId == 0)
                                {
                                    continue;
                                }
                                var result = from area in unitOfWork.Repository<Entities.Models.Area>().GetAll()
                                             join ter in unitOfWork.Repository<Entities.Models.Territory>().GetAll() on area.Id equals ter.AreaId
                                             join dis in unitOfWork.Repository<Entities.Models.Dealership>().GetAll() on ter.Id equals dis.TerritoryId
                                             where dis.DealershipTypeId == 1 && area.IsActive == true && ter.IsActive == true && dis.IsActive == true
                                                   && area.Id == lObjuserTerritory.AreaId
                                             select new
                                             {
                                                 dis.Id,
                                                 dis.Name,
                                                 dis.PhoneNo,
                                                 dis.Address,
                                                 dis.PinLocation
                                             };

                                var dealershipList = result.ToList();


                                foreach (var item in dealershipList)
                                {
                                    DealershipDetails lObjDealershipDetails = new DealershipDetails();
                                    lObjDealershipDetails.DealershipId = item.Id;
                                    lObjDealershipDetails.DealershipName = item.Name;
                                    lObjDealershipDetails.PhoneNo = item.PhoneNo;
                                    lObjDealershipDetails.Address = item.Address;
                                    lObjDealershipDetails.DealershipLocation = item.PinLocation;

                                    var distLat = JsonDocument.Parse(item.PinLocation).RootElement.GetProperty("lat").GetDouble();
                                    var distLng = JsonDocument.Parse(item.PinLocation).RootElement.GetProperty("lng").GetDouble();
                                    var distanceInMeters = 6371000 * Math.Acos(
                                                            Math.Cos(DegreeToRadian(command.lat)) * Math.Cos(DegreeToRadian(distLat)) *
                                                            Math.Cos(DegreeToRadian(distLng) - DegreeToRadian(command.lng)) +
                                                            Math.Sin(DegreeToRadian(command.lat)) * Math.Sin(DegreeToRadian(distLat)));



                                    lObjDealershipDetails.DistanceInMeters = distanceInMeters;

                                    // Conditional distance formatting
                                    lObjDealershipDetails.FormattedDistance = distanceInMeters < 1000 ? distanceInMeters.ToString("F0") : (distanceInMeters / 1000.0).ToString("F2");
                                    lObjDealershipDetails.FormattedDistanceUnit = distanceInMeters < 1000 ? "m" : "km";
                                    tokenModel.lstDealershipDetails.Add(lObjDealershipDetails);
                                }

                            }

                            tokenModel.lstDealershipDetails = tokenModel.lstDealershipDetails
                                                                .OrderBy(d => d.DistanceInMeters)  // Order by distance, nearest first
                                                                .ToList();
                        }

                    }
                    else
                    {
                        //Territory Level
                        var UserTerritory = from u in unitOfWork.Repository<UserTerritory>().GetAll()
                                            where u.IsActive == true && u.IsDelete == false && u.UserId == lObjUser.Id
                                            select u;

                        if (UserTerritory.Count() > 0)
                        {
                            tokenModel.lstDealershipDetails = new List<DealershipDetails>();
                            foreach (var lObjuserTerritory in UserTerritory)
                            {
                                if (lObjuserTerritory.TerritoryId == null || lObjuserTerritory.TerritoryId == 0)
                                {
                                    continue;
                                }
                                var result = from
                                             dis in unitOfWork.Repository<Entities.Models.Dealership>().GetAll()
                                             where dis.DealershipTypeId == 1 && dis.IsActive == true
                                                   && dis.TerritoryId == lObjuserTerritory.TerritoryId
                                             select new
                                             {
                                                 dis.Id,
                                                 dis.Name,
                                                 dis.PhoneNo,
                                                 dis.Address,
                                                 dis.PinLocation
                                             };

                                var dealershipList = result.ToList();


                                foreach (var item in dealershipList)
                                {
                                    DealershipDetails lObjDealershipDetails = new DealershipDetails();
                                    lObjDealershipDetails.DealershipId = item.Id;
                                    lObjDealershipDetails.DealershipName = item.Name;
                                    lObjDealershipDetails.PhoneNo = item.PhoneNo;
                                    lObjDealershipDetails.Address = item.Address;
                                    lObjDealershipDetails.DealershipLocation = item.PinLocation;

                                    var distLat = JsonDocument.Parse(item.PinLocation).RootElement.GetProperty("lat").GetDouble();
                                    var distLng = JsonDocument.Parse(item.PinLocation).RootElement.GetProperty("lng").GetDouble();
                                    var distanceInMeters = 6371000 * Math.Acos(
                                                            Math.Cos(DegreeToRadian(command.lat)) * Math.Cos(DegreeToRadian(distLat)) *
                                                            Math.Cos(DegreeToRadian(distLng) - DegreeToRadian(command.lng)) +
                                                            Math.Sin(DegreeToRadian(command.lat)) * Math.Sin(DegreeToRadian(distLat)));



                                    lObjDealershipDetails.DistanceInMeters = distanceInMeters;

                                    // Conditional distance formatting
                                    lObjDealershipDetails.FormattedDistance = distanceInMeters < 1000 ? distanceInMeters.ToString("F0") : (distanceInMeters / 1000.0).ToString("F2");
                                    lObjDealershipDetails.FormattedDistanceUnit = distanceInMeters < 1000 ? "m" : "km";
                                    tokenModel.lstDealershipDetails.Add(lObjDealershipDetails);
                                }

                            }

                            tokenModel.lstDealershipDetails = tokenModel.lstDealershipDetails
                                                                .OrderBy(d => d.DistanceInMeters)  // Order by distance, nearest first
                                                                .ToList();
                        }
                    }


                    if (lObjUser.Attachments.FirstOrDefault() != null && lObjUser.Attachments.Count() > 0)
                    {
                        string imageName = lObjUser.Attachments.FirstOrDefault()?.ImageName?.TrimStart('/') ?? "";

                        tokenModel.Image = GetImageAsBase64(Path.Combine(Localcontainer, imageName));
                    }
                    //var lObjuser = await unitOfWork.Repository<Entities.Models.AspNetUsers>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.Id == user.Id, null, null, "Attachments");
                    var lObjUserAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.UserId == lObjUser.Id && o.AttendanceDate.Date == DateTime.Now.Date);
                    if (lObjUserAttendance != null && lObjUserAttendance.Id > 0)
                    {
                        tokenModel.IsMarkAttendance = true;
                        tokenModel.IsPresent = lObjUserAttendance.IsPresent;
                        tokenModel.PresentTime = lObjUserAttendance.AttendanceDate;
                        tokenModel.IsCheckOut = lObjUserAttendance.CheckOut != null ? true : false;
                        tokenModel.CheckOutTime = lObjUserAttendance.CheckOut != null ? lObjUserAttendance.CheckOut : null;
                        tokenModel.AbsentReason = lObjUserAttendance.Reason;
                    }

                    tokenModel.IsLoginSuccess = true;
                    tokenModel.IsMobileDeviceRegister = tokenModel.IsMobileDeviceRegister == null ? false : tokenModel.IsMobileDeviceRegister;
                    tokenModel.IsAvailableForMobile = tokenModel.IsAvailableForMobile == null ? false : tokenModel.IsAvailableForMobile;
                    tokenModel.IsDistCompForAtten = tokenModel.IsDistCompForAtten == null ? false : tokenModel.IsDistCompForAtten;
                    tokenModel.IsLogedIn = tokenModel.IsLogedIn == null ? false : tokenModel.IsLogedIn;
                    //In Users k lye device registration nahi zaroori, Ye testing user hain or First 1 for google 
                    var allowedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                        {
                            "ase@kc.com",
                            "aseone@kc.com",
                            "asetwo@kc.com",
                            "asethree@kc.com",
                            "asefour@kc.com"
                        };
                    if (allowedEmails.Contains(tokenModel.Email.ToLower()))
                    {
                        tokenModel.IsMobileDeviceRegister = true;
                    }
                    return this.Result(ResponseStatus.OK, tokenModel, "User Data");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("SaveShopTagging")]
        public async Task<ActionResult<string>> SaveShopTagging(SaveShopTaggingCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (string.IsNullOrWhiteSpace(command.ImageFileSource))
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Shop Image is Compulsory");
                    }
                    if (command.lat == null || command.lat == 0 || command.lng == null || command.lng == 0)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Shop Location is Compulsory");
                    }

                    var lObjUserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.UserId == new Guid(command.UserId));

                    if (lObjUserTerritory == null)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Territory Not Found");
                    }

                    DateTime lDtOpeningTime = DateTime.ParseExact(command.OpeningTime, "h:mm tt", CultureInfo.InvariantCulture);
                    command.OpeningTime = lDtOpeningTime.ToString("HH:mm");

                    DateTime lDtClosingTime = DateTime.ParseExact(command.ClosingTime, "h:mm tt", CultureInfo.InvariantCulture);
                    command.ClosingTime = lDtClosingTime.ToString("HH:mm");

                    Shop lvarShop = new Shop();
                    lvarShop.CreatedById = new Guid(command.UserId);
                    lvarShop.CreatedDate = DateTime.Now;
                    lvarShop.Name = command.ShopName;
                    lvarShop.PhoneNo = command.PhoneNo;
                    lvarShop.OwnerName = command.OwnerName;
                    lvarShop.Address = command.Address;
                    lvarShop.PinLocation = GetJsonPinLocation((decimal)command.lat, (decimal)command.lng);
                    lvarShop.TerritoryId = (long)lObjUserTerritory.TerritoryId;
                    lvarShop.OpeningTime = TimeSpan.Parse(command.OpeningTime);
                    lvarShop.ClosingTime = TimeSpan.Parse(command.ClosingTime);
                    lvarShop.IsVerified = false;
                    lvarShop.IsTagFromMob = true;
                    lvarShop.ShopTypeId = command.ShopTypeId;
                    lvarShop.PepsiFridge = command.PepsiFridge;
                    lvarShop.CokeFridge = command.CokeFridge;
                    lvarShop.NestleFridge = command.NestleFridge;
                    lvarShop.NesfrutaFridge = command.NesfrutaFridge;
                    lvarShop.OthersFridge = command.OthersFridge;
                    lvarShop.Landmark = command.Landmark;
                    lvarShop.SecondaryPhoneNo = command.SecondaryPhoneNo;
                    unitOfWork.Repository<Entities.Models.Shop>().Add(lvarShop);
                    unitOfWork.SaveChanges();


                    Attachments attachment = new Attachments();
                    attachment.CreatedDate = DateTime.Now;
                    attachment.CreatedById = new Guid(command.UserId);
                    attachment.ShopId = lvarShop.Id;

                    BlobImageUploadModel blobModel = new()
                    {
                        File = "data:image/jpeg;base64," + command.ImageFileSource,
                        FileName = "ShopTagDSF" + command.ImageExtension,
                        FolderName = "assets/Files"
                    };

                    attachment.ImageName = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(blobModel, command.ImageExtension);
                    await unitOfWork.Repository<Attachments>().AddAsync(attachment);
                    unitOfWork.SaveChanges();
                    return this.Result(ResponseStatus.OK, null, "Shop Tagged Successfully!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("SaveShopTaggingByDistributorId")]
        public async Task<ActionResult<string>> SaveShopTaggingByDistributorId(SaveShopTaggingByDistCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (string.IsNullOrWhiteSpace(command.ImageFileSource))
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Shop Image is Compulsory");
                    }
                    if (command.lat == null || command.lat == 0 || command.lng == null || command.lng == 0)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Shop Location is Compulsory");
                    }
                    var lObjDistributor = await unitOfWork.Repository<Entities.Models.Dealership>().GetFirstAsNoTrackingAsync(o => o.DealershipTypeId == 1 && o.IsActive == true && o.Id == command.DistributorId);
                    if (lObjDistributor == null)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Distributor Not Found");
                    }


                    DateTime lDtOpeningTime = DateTime.ParseExact(command.OpeningTime, "h:mm tt", CultureInfo.InvariantCulture);
                    command.OpeningTime = lDtOpeningTime.ToString("HH:mm");

                    DateTime lDtClosingTime = DateTime.ParseExact(command.ClosingTime, "h:mm tt", CultureInfo.InvariantCulture);
                    command.ClosingTime = lDtClosingTime.ToString("HH:mm");

                    Shop lvarShop = new Shop();
                    lvarShop.CreatedById = new Guid(command.UserId);
                    lvarShop.CreatedDate = DateTime.Now;
                    lvarShop.Name = command.ShopName;
                    lvarShop.PhoneNo = command.PhoneNo;
                    lvarShop.OwnerName = command.OwnerName;
                    lvarShop.Address = command.Address;
                    lvarShop.PinLocation = GetJsonPinLocation((decimal)command.lat, (decimal)command.lng);
                    lvarShop.TerritoryId = (long)lObjDistributor.TerritoryId;
                    lvarShop.OpeningTime = TimeSpan.Parse(command.OpeningTime);
                    lvarShop.ClosingTime = TimeSpan.Parse(command.ClosingTime);
                    lvarShop.IsVerified = false;
                    lvarShop.IsTagFromMob = true;
                    lvarShop.ShopTypeId = command.ShopTypeId;
                    lvarShop.PepsiFridge = command.PepsiFridge;
                    lvarShop.CokeFridge = command.CokeFridge;
                    lvarShop.NestleFridge = command.NestleFridge;
                    lvarShop.NesfrutaFridge = command.NesfrutaFridge;
                    lvarShop.OthersFridge = command.OthersFridge;
                    lvarShop.Landmark = command.Landmark;
                    lvarShop.SecondaryPhoneNo = command.SecondaryPhoneNo;
                    lvarShop.StatusId = 2;
                    unitOfWork.Repository<Entities.Models.Shop>().Add(lvarShop);
                    unitOfWork.SaveChanges();


                    Attachments attachment = new Attachments();
                    attachment.CreatedDate = DateTime.Now;
                    attachment.CreatedById = new Guid(command.UserId);
                    attachment.ShopId = lvarShop.Id;

                    BlobImageUploadModel blobModel = new()
                    {
                        File = "data:image/jpeg;base64," + command.ImageFileSource,
                        FileName = "ShopTagDSF" + command.ImageExtension,
                        FolderName = "assets/Files/Shops"
                    };

                    attachment.ImageName = "/assets/Files/Shops/" + await blobService.UploadBase64FileToBlobAsync(blobModel, command.ImageExtension);
                    await unitOfWork.Repository<Attachments>().AddAsync(attachment);
                    unitOfWork.SaveChanges();
                    return this.Result(ResponseStatus.OK, null, "Shop Tagged Successfully!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("SaveShopTaggingByTerritoryId")]
        public async Task<ActionResult<string>> SaveShopTaggingByTerritoryId(SaveShopTaggingByTerritoryCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (command.ImageFileSource.Count() == 0)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Shop Image is Compulsory");
                    }
                    if (command.lat == null || command.lat == 0 || command.lng == null || command.lng == 0)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Shop Location is Compulsory");
                    }
                  

                    DateTime lDtOpeningTime = DateTime.ParseExact(command.OpeningTime, "h:mm tt", CultureInfo.InvariantCulture);
                    command.OpeningTime = lDtOpeningTime.ToString("HH:mm");

                    DateTime lDtClosingTime = DateTime.ParseExact(command.ClosingTime, "h:mm tt", CultureInfo.InvariantCulture);
                    command.ClosingTime = lDtClosingTime.ToString("HH:mm");

                    var shop = await unitOfWork.Repository<Entities.Models.Shop>().GetFirstAsync(x => x.Id == command.Id, null, null, "Attachments");
                    if (shop == null)
                    {
                        bool duplicationShop = await unitOfWork.Repository<Entities.Models.Shop>().GetExistsAsync(x => x.PhoneNo == command.PhoneNo && x.IsActive == true);
                        if (duplicationShop)
                        {
                            return this.Result(ResponseStatus.DuplicatePhoneNo, null, "Duplicate PhoneNo");
                        }

                        Shop lvarShop = new()
                        {
                            CreatedById = new Guid(command.UserId),
                            CreatedDate = DateTime.Now,
                            Name = command.ShopName,
                            PhoneNo = command.PhoneNo,
                            OwnerName = command.OwnerName,
                            Address = command.Address,
                            PinLocation = GetJsonPinLocation((decimal)command.lat, (decimal)command.lng),
                            TerritoryId = command.TerritoryId,
                            OpeningTime = TimeSpan.Parse(command.OpeningTime),
                            ClosingTime = TimeSpan.Parse(command.ClosingTime),
                            IsVerified = false,
                            IsTagFromMob = true,
                            ShopTypeId = command.ShopTypeId,
                            PepsiFridge = command.PepsiFridge,
                            CokeFridge = command.CokeFridge,
                            NestleFridge = command.NestleFridge,
                            NesfrutaFridge = command.NesfrutaFridge,
                            OthersFridge = command.OthersFridge,
                            Landmark = command.Landmark,
                            SecondaryPhoneNo = command.SecondaryPhoneNo,
                            StatusId = 2
                        };

                        unitOfWork.Repository<Shop>().Add(lvarShop);
                        unitOfWork.SaveChanges();

                        foreach (var item in command.ImageFileSource)
                        {
                            BlobImageUploadModel blobModel = new()
                            {
                                File = "data:image/jpeg;base64," + item,
                                FileName = "ShopTagDSF" + command.ImageExtension,
                                FolderName = "assets/Files/Shops"
                            };

                            Attachments attachment = new()
                            {
                                CreatedDate = DateTime.Now,
                                CreatedById = new Guid(command.UserId),
                                ShopId = lvarShop.Id,
                                ImageName = "/assets/Files/Shops/" + await blobService.UploadBase64FileToBlobAsync(blobModel, command.ImageExtension)
                            };

                            await unitOfWork.Repository<Attachments>().AddAsync(attachment);
                        }
                        unitOfWork.SaveChanges();
                    }
                    else
                    {
                        bool duplicationShopUpdate = await unitOfWork.Repository<Entities.Models.Shop>().GetExistsAsync(x => x.PhoneNo == command.PhoneNo && x.Id != command.Id && x.IsActive == true);
                        if (duplicationShopUpdate)
                        {
                            return this.Result(ResponseStatus.DuplicatePhoneNo, null, "Duplicate PhoneNo");
                        }
                        shop.Name = command.ShopName;
                        shop.PhoneNo = command.PhoneNo;
                        shop.OwnerName = command.OwnerName;
                        shop.Address = command.Address;
                        shop.PinLocation = GetJsonPinLocation((decimal)command.lat, (decimal)command.lng);
                        shop.TerritoryId = command.TerritoryId;
                        shop.OpeningTime = TimeSpan.Parse(command.OpeningTime);
                        shop.ClosingTime = TimeSpan.Parse(command.ClosingTime);
                        shop.IsVerified = false;
                        shop.IsTagFromMob = true;
                        shop.ShopTypeId = command.ShopTypeId;
                        shop.PepsiFridge = command.PepsiFridge;
                        shop.CokeFridge = command.CokeFridge;
                        shop.NestleFridge = command.NestleFridge;
                        shop.NesfrutaFridge = command.NesfrutaFridge;
                        shop.OthersFridge = command.OthersFridge;
                        shop.Landmark = command.Landmark;
                        shop.SecondaryPhoneNo = command.SecondaryPhoneNo;
                        shop.ModifiedById = new Guid(command.UserId);
                        shop.ModifiedDate = DateTime.Now;
                        shop.StatusId = 2;
                        unitOfWork.Repository<Shop>().Update(shop);
                        unitOfWork.SaveChanges();

                        if (shop.Attachments != null && shop.Attachments.Count > 0)
                        {
                            foreach (var item in shop.Attachments)
                            {
                                item.IsActive = false;
                                item.IsDelete = true;
                                item.DeleteDate = DateTime.Now;
                                item.ModifiedById = new Guid(command.UserId);
                                item.ModifiedDate = DateTime.Now;
                                unitOfWork.Repository<Attachments>().Update(item);
                                unitOfWork.SaveChanges();
                                await blobService.DeleteBlobDataAsync(item.ImageName);
                            }
                        }

                        foreach (var item in command.ImageFileSource)
                        {
                            BlobImageUploadModel blobModel = new()
                            {
                                File = "data:image/jpeg;base64," + item,
                                FileName = "ShopTagDSF" + command.ImageExtension,
                                FolderName = "assets/Files/Shops"
                            };

                            Attachments attachment = new()
                            {
                                CreatedDate = DateTime.Now,
                                CreatedById = new Guid(command.UserId),
                                ShopId = shop.Id,
                                ImageName = "/assets/Files/Shops/" + await blobService.UploadBase64FileToBlobAsync(blobModel, command.ImageExtension)
                            };

                            await unitOfWork.Repository<Attachments>().AddAsync(attachment);
                            unitOfWork.SaveChanges();
                        }

                    }

                    return this.Result(ResponseStatus.OK, null, "Shop Tagged Successfully!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("GetShopType")]
        public async Task<ActionResult<string>> GetShopType([FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var result = (
                              from st in unitOfWork.Repository<ShopType>().GetAll()
                              where st.IsActive == true
                                    && st.IsDelete == false
                              select new
                              {
                                  Id = st.Id,
                                  Name = st.Name
                              }
                          );

                    var QueryResult = result.ToList();

                    return this.Result(ResponseStatus.OK, QueryResult, QueryResult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("GetShopTaggingForVerificatiionBySupID")]
        public async Task<ActionResult<string>> GetShopTaggingForVerificatiionBySupID(UserAppDateCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var lObjUserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.UserId == new Guid(command.UserId));

                    if (lObjUserTerritory == null)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Territory Not Found");
                    }

                    var ASDTerritory = (from area in unitOfWork.Repository<Entities.Models.Area>().GetAll()
                                        join ter in unitOfWork.Repository<Entities.Models.Territory>().GetAll() on area.Id equals ter.AreaId
                                        where area.IsActive == true && ter.IsActive == true
                                              && area.Id == lObjUserTerritory.AreaId
                                        select ter.Id).Distinct();

                    var result = from sp in unitOfWork.Repository<Entities.Models.Shop>().GetAll()
                                 join u in unitOfWork.Repository<Entities.Models.AspNetUsers>().GetAll() on sp.CreatedById equals u.Id
                                 join t in unitOfWork.Repository<Entities.Models.Territory>().GetAll() on sp.TerritoryId equals t.Id
                                 join att in unitOfWork.Repository<Entities.Models.Attachments>().GetAll() on sp.Id equals att.ShopId into attGroup
                                 from att in attGroup.DefaultIfEmpty()  // Left join equivalent in LINQ
                                 where sp.IsActive == true
                                 && sp.IsVerified == false
                                 && sp.VerifiedById == null
                                  && ASDTerritory.Contains(sp.TerritoryId)
                                 orderby sp.Id descending
                                 select new
                                 {
                                     sp.Id,
                                     ShopName = sp.Name,
                                     sp.Address,
                                     sp.PhoneNo,
                                     sp.PinLocation,
                                     sp.TerritoryId,
                                     sp.SchedulerId,
                                     sp.CreatedById,
                                     sp.CreatedDate,
                                     sp.IsTagFromMob,
                                     sp.IsVerified,
                                     sp.OpeningTime,
                                     sp.ClosingTime,
                                     sp.OwnerName,
                                     ImageName = !string.IsNullOrEmpty(att?.ImageName)
                                                 && System.IO.File.Exists(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                                 ? GetImageAsBase64(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                                 : null,
                                     CreatedByName = u.FirstName + " " + u.LastName,
                                     TerritoryName = t.Name,
                                     TerritoryCoordinates = t.Coordinates,
                                 };

                    var shopListResult = result.Distinct().ToList();

                    return this.Result(ResponseStatus.OK, shopListResult, shopListResult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }
        [HttpPost]
        [Route("UpdateUVShopVerificationStatus")]
        public async Task<ActionResult<string>> UpdateUVShopVerificationStatus(UpdateUVShopVerificationStatusCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }

                    var lObjShops = await unitOfWork.Repository<Entities.Models.Shop>().GetFirstAsync(o => o.IsActive == true && o.Id == command.ShopId);

                    if (lObjShops == null)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Shop Not Found");
                    }

                    if (lObjShops.VerifiedById != null)
                    {
                        return this.Result(ResponseStatus.Conflict, null, "Shop Already Verified");
                    }

                    lObjShops.IsVerified = command.IsVerified;
                    lObjShops.VerifiedById = new Guid(command.UserId);
                    lObjShops.VerifiedDate = DateTime.Now;

                    unitOfWork.Repository<Shop>().Update(lObjShops);
                    unitOfWork.SaveChanges();

                    return this.Result(ResponseStatus.OK, null, "Shop Verification status has been updated");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("GetConfirmTerritoryShopByUserID")]
        public async Task<ActionResult<string>> GetConfirmTerritoryShopByUserID(UserAppDateCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var lObjUserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.UserId == new Guid(command.UserId));

                    if (lObjUserTerritory == null)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Territory Not Found");
                    }


                    var result = from sp in unitOfWork.Repository<Entities.Models.Shop>().GetAll()
                                 join u in unitOfWork.Repository<Entities.Models.AspNetUsers>().GetAll() on sp.CreatedById equals u.Id
                                 join t in unitOfWork.Repository<Entities.Models.Territory>().GetAll() on sp.TerritoryId equals t.Id
                                 join att in unitOfWork.Repository<Entities.Models.Attachments>().GetAll() on sp.Id equals att.ShopId into attGroup
                                 from att in attGroup.DefaultIfEmpty()  // Left join equivalent in LINQ
                                 where sp.IsActive == true
                                 && sp.IsVerified == true
                                    && (from ut in unitOfWork.Repository<Entities.Models.UserTerritory>().GetAll()
                                        where ut.UserId == new Guid(command.UserId)
                                              && ut.IsActive == true
                                        select ut.TerritoryId).Distinct().Contains(sp.TerritoryId)
                                 //orderby sp.Id descending
                                 orderby sp.VerifiedDate descending
                                 select new
                                 {
                                     sp.Id,
                                     ShopName = sp.Name,
                                     sp.Address,
                                     sp.PhoneNo,
                                     sp.PinLocation,
                                     sp.TerritoryId,
                                     sp.SchedulerId,
                                     sp.CreatedById,
                                     sp.CreatedDate,
                                     sp.IsTagFromMob,
                                     sp.IsVerified,
                                     sp.VerifiedDate,
                                     sp.OpeningTime,
                                     sp.ClosingTime,
                                     sp.OwnerName,
                                     ImageName = !string.IsNullOrEmpty(att?.ImageName)
                                                 && System.IO.File.Exists(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                                 ? GetImageAsBase64(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                                 : null,
                                     CreatedByName = u.FirstName + " " + u.LastName,
                                     TerritoryName = t.Name,
                                     TerritoryCoordinates = t.Coordinates,
                                 };

                    var shopListResult = result.ToList();


                    return this.Result(ResponseStatus.OK, shopListResult, shopListResult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("GetNonProductiveTerritoryShopByUserID")]
        public async Task<ActionResult<string>> GetNonProductiveTerritoryShopByUserID(UserAppDateCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var lObjUserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.UserId == new Guid(command.UserId));

                    if (lObjUserTerritory == null)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Territory Not Found");
                    }

                    var result = (from sp in unitOfWork.Repository<Shop>().GetAll()
                                  join u in unitOfWork.Repository<AspNetUsers>().GetAll() on sp.CreatedById equals u.Id
                                  join t in unitOfWork.Repository<Entities.Models.Territory>().GetAll() on sp.TerritoryId equals t.Id
                                  join att in unitOfWork.Repository<Attachments>().GetAll().DefaultIfEmpty() on sp.Id equals att.ShopId
                                  join ord in unitOfWork.Repository<Entities.Models.Order>().GetAll().Where(o => o.IsActive == true).DefaultIfEmpty() on sp.Id equals ord.ShopId into orders
                                  from ord in orders.DefaultIfEmpty() // Correct handling of left join with orders
                                  where ord == null // Ensuring no active orders exist
                                        && sp.IsActive == true
                                        && sp.IsVerified == true
                                        && (from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                                            where ut.UserId == new Guid(command.UserId)
                                                  && ut.IsActive == true
                                            select ut.TerritoryId).Distinct().Contains(sp.TerritoryId)
                                  orderby sp.Id descending
                                  select new
                                  {
                                      sp.Id,
                                      ShopName = sp.Name,
                                      sp.Address,
                                      sp.PhoneNo,
                                      sp.PinLocation,
                                      sp.TerritoryId,
                                      sp.SchedulerId,
                                      sp.CreatedById,
                                      sp.CreatedDate,
                                      sp.IsTagFromMob,
                                      sp.IsVerified,
                                      sp.VerifiedDate,
                                      sp.OpeningTime,
                                      sp.ClosingTime,
                                      sp.OwnerName,
                                      ImageName = !string.IsNullOrEmpty(att?.ImageName)
                                                 && System.IO.File.Exists(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                                 ? GetImageAsBase64(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                                 : null,
                                      CreatedByName = u.FirstName + " " + u.LastName,
                                      TerritoryName = t.Name,
                                      TerritoryCoordinates = t.Coordinates,
                                  }).Distinct().ToList();

                    var shopListResult = result.ToList();

                    return this.Result(ResponseStatus.OK, shopListResult, shopListResult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("GetProductiveTerritoryShopByUserID")]
        public async Task<ActionResult<string>> GetProductiveTerritoryShopByUserID(UserAppDateCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var lObjUserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.UserId == new Guid(command.UserId));

                    if (lObjUserTerritory == null)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Territory Not Found");
                    }

                    var result = (from sp in unitOfWork.Repository<Shop>().GetAll()
                                  join u in unitOfWork.Repository<AspNetUsers>().GetAll() on sp.CreatedById equals u.Id
                                  join t in unitOfWork.Repository<Entities.Models.Territory>().GetAll() on sp.TerritoryId equals t.Id
                                  join att in unitOfWork.Repository<Attachments>().GetAll().DefaultIfEmpty() on sp.Id equals att.ShopId
                                  join ord in unitOfWork.Repository<Entities.Models.Order>().GetAll().Where(o => o.IsActive == true).DefaultIfEmpty() on sp.Id equals ord.ShopId into orders
                                  from ord in orders.DefaultIfEmpty() // Correct handling of left join with orders
                                  where ord != null // Ensuring no active orders exist
                                        && ord.IsActive == true
                                        && sp.IsActive == true
                                        && sp.IsVerified == true
                                        && (from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                                            where ut.UserId == new Guid(command.UserId)
                                                  && ut.IsActive == true
                                            select ut.TerritoryId).Distinct().Contains(sp.TerritoryId)
                                  orderby sp.Id descending
                                  select new
                                  {
                                      sp.Id,
                                      ShopName = sp.Name,
                                      sp.Address,
                                      sp.PhoneNo,
                                      sp.PinLocation,
                                      sp.TerritoryId,
                                      sp.SchedulerId,
                                      sp.CreatedById,
                                      sp.CreatedDate,
                                      sp.IsTagFromMob,
                                      sp.IsVerified,
                                      sp.VerifiedDate,
                                      sp.OpeningTime,
                                      sp.ClosingTime,
                                      sp.OwnerName,
                                      ImageName = !string.IsNullOrEmpty(att?.ImageName)
                                                 && System.IO.File.Exists(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                                 ? GetImageAsBase64(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                                 : null,
                                      CreatedByName = u.FirstName + " " + u.LastName,
                                      TerritoryName = t.Name,
                                      TerritoryCoordinates = t.Coordinates,
                                  }).Distinct().ToList();

                    var shopListResult = result.ToList();

                    return this.Result(ResponseStatus.OK, shopListResult, shopListResult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("GetRejectedVerifyStatusByUserID")]
        public async Task<ActionResult<string>> GetRejectedVerifyStatusByUserID(UserAppDateCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var lObjUserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.UserId == new Guid(command.UserId));

                    if (lObjUserTerritory == null)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Territory Not Found");
                    }

                    var result = (from sp in unitOfWork.Repository<Shop>().GetAll()
                                  join u in unitOfWork.Repository<AspNetUsers>().GetAll() on sp.CreatedById equals u.Id
                                  join t in unitOfWork.Repository<Entities.Models.Territory>().GetAll() on sp.TerritoryId equals t.Id
                                  join att in unitOfWork.Repository<Attachments>().GetAll().DefaultIfEmpty() on sp.Id equals att.ShopId
                                  where sp.IsActive == true
                                        && sp.IsVerified == false
                                        && sp.VerifiedById == new Guid(command.UserId)
                                  //orderby sp.Id descending
                                  orderby sp.VerifiedDate descending
                                  select new
                                  {
                                      sp.Id,
                                      ShopName = sp.Name,
                                      sp.Address,
                                      sp.PhoneNo,
                                      sp.PinLocation,
                                      sp.TerritoryId,
                                      sp.SchedulerId,
                                      sp.CreatedById,
                                      sp.CreatedDate,
                                      sp.IsTagFromMob,
                                      sp.IsVerified,
                                      sp.VerifiedDate,
                                      sp.OpeningTime,
                                      sp.ClosingTime,
                                      sp.OwnerName,
                                      ImageName = !string.IsNullOrEmpty(att?.ImageName)
                                                 && System.IO.File.Exists(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                                 ? GetImageAsBase64(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                                 : null,
                                      CreatedByName = u.FirstName + " " + u.LastName,
                                      TerritoryName = t.Name,
                                      TerritoryCoordinates = t.Coordinates,
                                  }).ToList();

                    var shopListResult = result.ToList();

                    return this.Result(ResponseStatus.OK, shopListResult, shopListResult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("GetUserShopTagHistoryByUserId")]
        public async Task<ActionResult<string>> GetUserShopTagHistoryByUserId(UserShopTagHistoryByUserCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var lObjUserTerritory = await unitOfWork.Repository<Entities.Models.UserTerritory>().GetFirstAsNoTrackingAsync(o => o.IsActive == true && o.UserId == new Guid(command.UserId));

                    if (lObjUserTerritory == null)
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Territory Not Found");
                    }

                    //var result = (from sp in unitOfWork.Repository<Shop>().GetAll()
                    //              join status in unitOfWork.Repository<Status>().GetAll() on sp.StatusId equals status.Id
                    //              join u in unitOfWork.Repository<AspNetUsers>().GetAll() on sp.CreatedById equals u.Id
                    //              join att in unitOfWork.Repository<Attachments>().GetAll().DefaultIfEmpty() on sp.Id equals att.ShopId
                    //              where sp.IsActive == true && sp.CreatedById == new Guid(command.UserId) && att.IsActive == true
                    //              orderby sp.Id descending
                    //              select new
                    //              {
                    //                  sp.Id,
                    //                  ShopName = sp.Name,
                    //                  sp.Address,
                    //                  sp.PhoneNo,
                    //                  sp.SecondaryPhoneNo,
                    //                  sp.ShopTypeId,
                    //                  sp.PinLocation,
                    //                  sp.TerritoryId,
                    //                  sp.SchedulerId,
                    //                  sp.CreatedById,
                    //                  sp.VerifiedDate,
                    //                  sp.Landmark,
                    //                  sp.PepsiFridge,
                    //                  sp.CokeFridge,
                    //                  sp.NesfrutaFridge,
                    //                  sp.NestleFridge,
                    //                  sp.OthersFridge,
                    //                  CreatedDate = sp.CreatedDate.HasValue ? sp.CreatedDate.Value.ToString("dd MMMM yyyy, hh:mm tt") : null,
                    //                  sp.IsTagFromMob,
                    //                  VerificationStatus = sp.IsVerified == true ? "Confirmed" :
                    //                                      sp.IsVerified == false && sp.VerifiedById == null ? "Pending" :
                    //                                      sp.IsVerified == false && sp.VerifiedById != null ? "ed" : "Unknown",
                    //                  OpeningTime = sp.OpeningTime.HasValue
                    //                                  ? string.Format("{0:hh:mm tt}", DateTime.Today.Add(sp.OpeningTime.Value)) // Convert to 12-hour format
                    //                                  : null, // Handle null case
                    //                  ClosingTime = sp.ClosingTime.HasValue
                    //                                  ? string.Format("{0:hh:mm tt}", DateTime.Today.Add(sp.ClosingTime.Value)) // Convert to 12-hour format
                    //                                  : null, // Handle null case
                    //                  sp.OwnerName,
                    //                  ImageName = !string.IsNullOrEmpty(att?.ImageName)
                    //                             && System.IO.File.Exists(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                    //                             ? GetImageAsBase64(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                    //                             : null,
                    //                  ImageNamea = att?.ImageName,
                    //                  CreatedByName = u.FirstName + " " + u.LastName,
                    //                  Status = sp.StatusId != null ? status.Title : "",
                    //                  sp.Remarks
                    //              }).ToList();

                    //var shopListResult = result.ToList();
                    //return this.Result(ResponseStatus.OK, shopListResult, shopListResult.Count().ToString());

                    // Build the base query
                    var query = from sp in unitOfWork.Repository<Shop>().GetAll()
                                join status in unitOfWork.Repository<Status>().GetAll() on sp.StatusId equals status.Id
                                join u in unitOfWork.Repository<AspNetUsers>().GetAll() on sp.CreatedById equals u.Id
                                join att in unitOfWork.Repository<Attachments>().GetAll().DefaultIfEmpty() on sp.Id equals att.ShopId
                                where sp.IsActive == true
                                    && sp.CreatedById == new Guid(command.UserId)
                                    && att.IsActive == true
                                orderby sp.Id descending
                                select new
                                {
                                    sp.Id,
                                    ShopName = sp.Name,
                                    sp.Address,
                                    sp.PhoneNo,
                                    sp.SecondaryPhoneNo,
                                    sp.ShopTypeId,
                                    sp.PinLocation,
                                    sp.TerritoryId,
                                    sp.SchedulerId,
                                    sp.CreatedById,
                                    sp.VerifiedDate,
                                    sp.Landmark,
                                    sp.PepsiFridge,
                                    sp.CokeFridge,
                                    sp.NesfrutaFridge,
                                    sp.NestleFridge,
                                    sp.OthersFridge,
                                    CreatedDate = sp.CreatedDate.HasValue ? sp.CreatedDate.Value.ToString("dd MMMM yyyy, hh:mm tt") : null,
                                    sp.IsTagFromMob,
                                    VerificationStatus = sp.IsVerified == true ? "Confirmed" :
                                                         sp.IsVerified == false && sp.VerifiedById == null ? "Pending" :
                                                         sp.IsVerified == false && sp.VerifiedById != null ? "ed" : "Unknown",
                                    OpeningTime = sp.OpeningTime.HasValue
                                                    ? string.Format("{0:hh:mm tt}", DateTime.Today.Add(sp.OpeningTime.Value))
                                                    : null,
                                    ClosingTime = sp.ClosingTime.HasValue
                                                    ? string.Format("{0:hh:mm tt}", DateTime.Today.Add(sp.ClosingTime.Value))
                                                    : null,
                                    sp.OwnerName,
                                    ImageName = !string.IsNullOrEmpty(att?.ImageName)
                                               && System.IO.File.Exists(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                               ? GetImageAsBase64(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                               : null,
                                    ImageNamea = att?.ImageName,
                                    CreatedByName = u.FirstName + " " + u.LastName,
                                    Status = sp.StatusId != null ? status.Title : "",
                                    sp.Remarks
                                };

                    // Apply Paging
                    if (command.PagingData != null && command.PagingData.IsPagingEnabled)
                    {
                        query = query
                            .Skip(command.PagingData.Skip)
                            .Take(command.PagingData.Take);
                    }

                    // Execute query
                    var shopListResult = query.ToList();
                    // Return result
                    return this.Result(ResponseStatus.OK, shopListResult, shopListResult.Count.ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }
        private static double DegreeToRadian(double degree)
        {
            return degree * (Math.PI / 180);
        }
        //public static string GetImageAsBase64(string imagePath)
        //{
        //    if (string.IsNullOrWhiteSpace(imagePath) || !System.IO.File.Exists(imagePath))
        //        return null;

        //    try
        //    {
        //        FileInfo fileInfo = new FileInfo(imagePath);
        //        long fileSizeInKB = fileInfo.Length / 1024;

        //        byte[] imageBytes;

        //        if (fileSizeInKB > 200)
        //        {
        //            using (var originalImage = Image.FromFile(imagePath))
        //            {
        //                int newWidth = Math.Max(1, originalImage.Width / 3);
        //                int newHeight = Math.Max(1, originalImage.Height / 3);

        //                using (var resizedImage = new Bitmap(originalImage, new Size(newWidth, newHeight)))
        //                using (var ms = new MemoryStream())
        //                {
        //                    var qualityParam = new EncoderParameter(Encoder.Quality, 50L);
        //                    var jpegCodec = GetEncoder(ImageFormat.Jpeg);
        //                    if (jpegCodec == null)
        //                    {
        //                        Console.WriteLine("JPEG encoder not found.");
        //                        return null;
        //                    }

        //                    var encoderParams = new EncoderParameters(1);
        //                    encoderParams.Param[0] = qualityParam;

        //                    resizedImage.Save(ms, jpegCodec, encoderParams);
        //                    imageBytes = ms.ToArray();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            imageBytes = System.IO.File.ReadAllBytes(imagePath);
        //        }

        //        string mimeType = GetMimeType(imagePath);
        //        return $"data:{mimeType};base64,{Convert.ToBase64String(imageBytes)}";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ($"GetImageAsBase64 failed for path: {imagePath} - Error: {ex.Message}");
        //        //return null;
        //    }
        //}

        public static string GetImageAsBase64(string imagePath)
        {
            try
            {
                if (!System.IO.File.Exists(imagePath))
                    return null;

                var fileInfo = new FileInfo(imagePath);
                string mime = GetMimeType(imagePath);

                // If image is already ≤ 20 KB, return as-is
                if (fileInfo.Length <= 20 * 1024)
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(imagePath);
                    return $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
                }

                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(imagePath)) // Use Rgba32 for transparency-aware handling
                {
                    // Draw on white background to remove black artifacts from transparent PNGs
                    var whiteBackground = new Image<Rgba32>(image.Width, image.Height);
                    whiteBackground.Mutate(ctx =>
                    {
                        ctx.Fill(SixLabors.ImageSharp.Color.White);
                        ctx.DrawImage(image, 1f);
                    });

                    // Resize if too large
                    if (whiteBackground.Width > 2000 || whiteBackground.Height > 2000)
                    {
                        whiteBackground.Mutate(x =>
                            x.Resize(whiteBackground.Width / 2, whiteBackground.Height / 2));
                    }

                    using (var ms = new MemoryStream())
                    {
                        whiteBackground.Save(ms, new JpegEncoder
                        {
                            Quality = 70
                        });

                        string base64 = Convert.ToBase64String(ms.ToArray());
                        return $"data:image/jpeg;base64,{base64}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Compress Fallback] Failed: {imagePath} - {ex.Message}");
                return null;
            }
        }


        //public static string GetImageAsBase64(string imagePath)
        //{
        //    if (!System.IO.File.Exists(imagePath))
        //    {
        //        return "Image Not Found";
        //    }

        //    FileInfo fileInfo = new FileInfo(imagePath);
        //    long fileSizeInKB = fileInfo.Length / 1024;

        //    byte[] imageBytes;

        //    if (fileSizeInKB > 200)
        //    {
        //        // Compress/Resize the image
        //        using (var originalImage = Image.FromFile(imagePath))
        //        {
        //            int newWidth = originalImage.Width / 3; // reduce width by 50%
        //            int newHeight = originalImage.Height / 3; // reduce height by 50%

        //            using (var resizedImage = new Bitmap(originalImage, new Size(newWidth, newHeight)))
        //            {
        //                using (var ms = new MemoryStream())
        //                {
        //                    var qualityParam = new EncoderParameter(Encoder.Quality, 50L); // Quality 75%
        //                    var jpegCodec = GetEncoder(ImageFormat.Jpeg);
        //                    var encoderParams = new EncoderParameters(1);
        //                    encoderParams.Param[0] = qualityParam;

        //                    resizedImage.Save(ms, jpegCodec, encoderParams);
        //                    imageBytes = ms.ToArray();
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // No compression needed
        //        imageBytes = System.IO.File.ReadAllBytes(imagePath);
        //    }

        //    string mimeType = GetMimeType(imagePath);
        //    string base64String = Convert.ToBase64String(imageBytes);
        //    return $"data:{mimeType};base64,{base64String}";
        //}

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        private static string GetMimeType(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream",
            };
        }
        [HttpPost]
        [Route("GetTodayDSFShopByUserLocation")]
        public async Task<ActionResult<string>> GetDSFShopByUserLocation(GetTodayDSFShopByUserLocationCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }

                    // Assuming you have the necessary repositories in place
                    var currentLat = command.lat;
                    var currentLng = command.lng;
                    string TodayDay = DateTime.Now.DayOfWeek.ToString();
                    var result = (
                                  from dfr in unitOfWork.Repository<DSFRoute>().GetAll()
                                  join usr in unitOfWork.Repository<AspNetUsers>().GetAll() on dfr.DSFId equals usr.Id
                                  join url in unitOfWork.Repository<AspNetUserRoles>().GetAll() on usr.Id equals url.UserId
                                  join rol in unitOfWork.Repository<AspNetRoles>().GetAll() on url.RoleId equals rol.Id
                                  join rt in unitOfWork.Repository<Route>().GetAll() on dfr.RouteId equals rt.Id
                                  join rs in unitOfWork.Repository<ShopRouteFrequency>().GetAll() on rt.Id equals rs.RouteId
                                  join sp in unitOfWork.Repository<Shop>().GetAll() on rs.ShopId equals sp.Id

                                  // Left join with MarkShopVisit
                                  join msv in unitOfWork.Repository<MarkShopVisit>().GetAll()
                                              .Where(msv => msv.IsActive && msv.CreatedDate.HasValue && msv.CreatedDate.Value.Date == DateTime.Now.Date)
                                              on sp.Id equals msv.ShopId into msGroup // Perform LEFT JOIN by grouping
                                  from msv in msGroup.DefaultIfEmpty() // Use DefaultIfEmpty for LEFT JOIN behavior


                                  join ord in unitOfWork.Repository<Entities.Models.Order>().GetAll()
                                              .Where(ord => ord.IsActive && ord.CreatedDate.HasValue && ord.CreatedDate.Value.Date == DateTime.Now.Date)
                                              on sp.Id equals ord.ShopId into ordGroup // Perform LEFT JOIN by grouping
                                  from ord in ordGroup.DefaultIfEmpty() // Use DefaultIfEmpty for LEFT JOIN behavior

                                  where
                                  ((TodayDay == "Monday" && rs.Monday == true) || (TodayDay == "Tuesday" && rs.Tuesday == true) || (TodayDay == "Wednesday" && rs.Wednesday == true) || (TodayDay == "Thursday" && rs.Thursday == true)
                                  || (TodayDay == "Friday" && rs.Friday == true) || (TodayDay == "Saturday" && rs.Saturday == true) || (TodayDay == "Sunday" && rs.Sunday == true)) &&
                                  usr.IsActive && rol.IsActive && dfr.IsActive && rt.IsActive && rs.IsActive && sp.IsActive
                                        && dfr.DSFId == new Guid(command.UserId)
                                  //&& rt.VisitDay == DateTime.Now.DayOfWeek.ToString() // Assuming VisitDay is a string in database
                                  let shopLat = JsonDocument.Parse(sp.PinLocation).RootElement.GetProperty("lat").GetDouble()
                                  let shopLng = JsonDocument.Parse(sp.PinLocation).RootElement.GetProperty("lng").GetDouble()
                                  let distanceInMeters = 6371000 * Math.Acos(
                                      Math.Cos(DegreeToRadian(currentLat)) * Math.Cos(DegreeToRadian(shopLat)) *
                                      Math.Cos(DegreeToRadian(shopLng) - DegreeToRadian(currentLng)) +
                                      Math.Sin(DegreeToRadian(currentLat)) * Math.Sin(DegreeToRadian(shopLat))
                                  )
                                  orderby msv == null ? 0 : 1, ord == null ? 0 : 1, distanceInMeters
                                  //orderby  distanceInMeters
                                  select new
                                  {
                                      usr.Email,
                                      UserFullName = usr.FirstName + " " + usr.LastName,
                                      RoleName = rol.Name,
                                      RoleDesc = rol.Description,
                                      RouteName = rt.Name,
                                      //rt.VisitDay,
                                      ShopId = sp.Id,
                                      ShopName = sp.Name,
                                      sp.OwnerName,
                                      ShopAddress = sp.Address,
                                      sp.PinLocation,
                                      sp.OpeningTime,
                                      sp.ClosingTime,
                                      sp.PhoneNo,
                                      ShopLat = shopLat,
                                      ShopLng = shopLng,
                                      DistanceInMeters = distanceInMeters,

                                      // Conditional distance formatting
                                      FormattedDistance = distanceInMeters < 1000
                                          ? distanceInMeters.ToString("F0")
                                          : (distanceInMeters / 1000.0).ToString("F2"),
                                      FormattedDistanceUnit = distanceInMeters < 1000
                                          ? "m"
                                          : "km",
                                      // Check if the shop has been visited
                                      IsVisited = msv != null ? "Yes" : "No",
                                      VisitId = msv?.Id, // Safely access with null conditional operator

                                      // Check if there's an order for the shop
                                      IsOrder = ord != null ? "Yes" : "No",
                                      OrderId = ord?.Id // Safely access with null conditional operator
                                  }
                              ).ToList();




                    var shopListResult = result.ToList();

                    return this.Result(ResponseStatus.OK, shopListResult, shopListResult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("MarkShopVisit")]
        public async Task<ActionResult<string>> MarkShopVisit(MarkShopVisitCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    var result = await mediator.Send(command);
                    if (result == 200)
                    {
                        return this.Result(ResponseStatus.OK, null, "Visit Marked Successfully");
                    }
                    else if (result == 409)
                    {
                        return this.Result(ResponseStatus.Conflict, null, "Visit Marked Already!");
                    }
                    else
                    {
                        return this.Result(ResponseStatus.Error, null, "Something went wrong!");
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("GetMarkShopVisitsById")]
        public async Task<ActionResult<string>> GetMarkShopVisitsById([FromQuery] long markShopVisitsId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }



                    var result = (from ms in unitOfWork.Repository<MarkShopVisit>().GetAll()
                                  join att in unitOfWork.Repository<Attachments>().GetAll() on ms.Id equals att.MarkShopVisitId
                                  join usr in unitOfWork.Repository<AspNetUsers>().GetAll() on ms.CreatedById equals usr.Id
                                  where ms.IsActive == true && att.IsActive == true && ms.Id == markShopVisitsId
                                  select new
                                  {
                                      ms.IsOpen,
                                      ms.Comments,
                                      VisitOn = ms.CreatedDate,
                                      VisitBy = usr.FirstName + " " + usr.LastName,
                                      ImageName = !string.IsNullOrEmpty(att?.ImageName)
                                                 && System.IO.File.Exists(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                                 ? GetImageAsBase64(Path.Combine(Localcontainer, att.ImageName.TrimStart('/')))
                                                 : null,
                                  }).ToList();



                    var markShopVisitsListResult = result.ToList();


                    return this.Result(ResponseStatus.OK, markShopVisitsListResult, markShopVisitsListResult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("GetTodayMyTeamStatusByUserId")]
        public async Task<ActionResult<string>> GetTodayMyTeamStatusByUserId([FromQuery] string userId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var today = DateTime.Today;

                    //var result = (from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                    //              join usr in unitOfWork.Repository<AspNetUsers>().GetAll() on ut.UserId equals usr.Id
                    //              join url in unitOfWork.Repository<AspNetUserRoles>().GetAll() on usr.Id equals url.UserId
                    //              join rol in unitOfWork.Repository<AspNetRoles>().GetAll() on url.RoleId equals rol.Id
                    //              join att in unitOfWork.Repository<Attachments>().GetAll().Where(a => a.IsActive).DefaultIfEmpty() on usr.Id equals att.UserId into attGroup
                    //              from att in attGroup.DefaultIfEmpty()
                    //              where (from subUt in unitOfWork.Repository<UserTerritory>().GetAll()
                    //                     where subUt.UserId == new Guid(userId) && subUt.IsActive
                    //                     select subUt.TerritoryId).Distinct().Contains(ut.TerritoryId)
                    //                    && ut.IsActive
                    //                    && ut.UserId != new Guid(userId)
                    //                    && rol.IsActive
                    //              select new
                    //              {
                    //                  FullName = usr.FirstName + " " + usr.LastName,
                    //                  usr.PhoneNumber,
                    //                  RoleName = rol.Name,
                    //                  PresentStatus = "Offline",
                    //                  Reason = string.Empty,
                    //                  AttendanceDate = string.Empty,
                    //                  att?.ImageName
                    //              }).ToList();

                    var result = (from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                                  join usr in unitOfWork.Repository<AspNetUsers>().GetAll() on ut.UserId equals usr.Id
                                  join url in unitOfWork.Repository<AspNetUserRoles>().GetAll() on usr.Id equals url.UserId
                                  join rol in unitOfWork.Repository<AspNetRoles>().GetAll() on url.RoleId equals rol.Id
                                  join att in unitOfWork.Repository<Attachments>().GetAll().Where(a => a.IsActive).DefaultIfEmpty() on usr.Id equals att.UserId into attGroup
                                  from att in attGroup.DefaultIfEmpty()
                                  join uat in unitOfWork.Repository<UserAttendance>().GetAll()
                                                .Where(a => a.IsActive && a.AttendanceDate.Date == DateTime.Now.Date).DefaultIfEmpty()
                                                on usr.Id equals (uat == null ? null : uat.UserId) into uatGroup
                                  from uat in uatGroup.DefaultIfEmpty()
                                  where (from subUt in unitOfWork.Repository<UserTerritory>().GetAll()
                                         where subUt.UserId == new Guid(userId) && subUt.IsActive
                                         select subUt.TerritoryId).Distinct().Contains(ut.TerritoryId)
                                        && ut.IsActive
                                        && ut.UserId != new Guid(userId)
                                        && rol.IsActive
                                  select new
                                  {
                                      FullName = usr.FirstName + " " + usr.LastName,
                                      usr.PhoneNumber,
                                      RoleName = rol.Name,
                                      PresentStatus = uat == null ? "Offline" :
                                                      ((bool)uat.IsPresent ? "Present" : "Leave"),
                                      Reason = uat == null ? string.Empty : uat.Reason,
                                      AttendanceDate = uat == null ? (DateTime?)null : uat.AttendanceDate,
                                      ImageName = !string.IsNullOrEmpty(att?.ImageName)
                                                  && System.IO.File.Exists(Path.Combine(Localcontainer, att?.ImageName.TrimStart('/')))
                                                  ? GetImageAsBase64(Path.Combine(Localcontainer, att?.ImageName.TrimStart('/')))
                                                  : null
                                  }).ToList();

                    //var shopListResult = result.ToList();                    
                    var getTodayMyTeamStatusByUserIdresult = result.ToList();

                    return this.Result(ResponseStatus.OK, getTodayMyTeamStatusByUserIdresult, getTodayMyTeamStatusByUserIdresult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("GetTodayRouteAllShopCountBySupervisorId")]
        public async Task<ActionResult<string>> GetTodayRouteAllShopCountBySupervisorId([FromQuery] string userId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var today = DateTime.Today;

                    var todayVisitDay = DateTime.Now.ToString("dddd");

                    var activeTerritoryIds = unitOfWork.Repository<UserTerritory>().GetAll()
                        .Where(subUt => subUt.UserId == new Guid(userId) && subUt.IsActive)
                        .Select(subUt => subUt.TerritoryId)
                        .Distinct()
                        .ToList(); // Get the list of active territory IDs

                    var result =
                        from ut in unitOfWork.Repository<UserTerritory>().GetAll().Where(x => x.IsActive)
                        join tr in unitOfWork.Repository<Territory>().GetAll().Where(x => x.IsActive) on ut.TerritoryId equals tr.Id
                        join usr in unitOfWork.Repository<AspNetUsers>().GetAll().Where(x => x.IsActive) on ut.UserId equals usr.Id
                        join url in unitOfWork.Repository<AspNetUserRoles>().GetAll() on usr.Id equals url.UserId
                        join rol in unitOfWork.Repository<AspNetRoles>().GetAll()
                            .Where(x => x.IsActive && x.Name == "DSF") on url.RoleId equals rol.Id
                        join dsf in unitOfWork.Repository<DSFRoute>().GetAll().Where(x => x.IsActive) on ut.UserId equals dsf.DSFId
                        join rt in unitOfWork.Repository<Route>().GetAll()
                            .Where(x => x.IsActive
                            //&& x.VisitDay == todayVisitDay
                            ) on dsf.RouteId equals rt.Id
                        join rts in unitOfWork.Repository<RouteShop>().GetAll().Where(x => x.IsActive) on rt.Id equals rts.RouteId
                        join sp in unitOfWork.Repository<Shop>().GetAll().Where(x => x.IsActive) on rts.ShopId equals sp.Id
                        join att in unitOfWork.Repository<Attachments>().GetAll().Where(x => x.IsActive) on usr.Id equals att.UserId into attGroup
                        from att in attGroup.DefaultIfEmpty() // Left join for attachments
                        where activeTerritoryIds.Contains(ut.TerritoryId) // Filters for active territories
                        group rts by new
                        {
                            TerritoryId = tr.Id,
                            TerritoryName = tr.Name,
                            RouteId = rt.Id,
                            RouteName = rt.Name,
                            DSFName = usr.FirstName + " " + usr.LastName,
                            RoleName = rol.Name,
                            UserImage = att.ImageName
                        } into grouped
                        select new
                        {
                            TerritoryId = grouped.Key.TerritoryId,
                            TerritoryName = grouped.Key.TerritoryName,
                            RouteId = grouped.Key.RouteId,
                            RouteName = grouped.Key.RouteName,
                            DSFName = grouped.Key.DSFName,
                            RoleName = grouped.Key.RoleName,
                            UserImage = !string.IsNullOrEmpty(grouped.Key.UserImage)
                            && System.IO.File.Exists(Path.Combine(Localcontainer, grouped.Key.UserImage.TrimStart('/')))
                            ? GetImageAsBase64(Path.Combine(Localcontainer, grouped.Key.UserImage.TrimStart('/')))
                            : null,
                            NoOfShop = grouped.Count() // Count the number of shops per route
                        };


                    var getTodayMyTeamStatusByUserIdresult = result.ToList();

                    return this.Result(ResponseStatus.OK, getTodayMyTeamStatusByUserIdresult, getTodayMyTeamStatusByUserIdresult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("GetTodayRouteVisitedShopCountBySupervisorId")]
        public async Task<ActionResult<string>> GetTodayRouteVisitedShopCountBySupervisorId([FromQuery] string userId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var today = DateTime.Today;

                    var todayVisitDay = DateTime.Now.ToString("dddd");

                    var activeTerritoryIds = unitOfWork.Repository<UserTerritory>().GetAll()
                        .Where(subUt => subUt.UserId == new Guid(userId) && subUt.IsActive)
                        .Select(subUt => subUt.TerritoryId)
                        .Distinct()
                        .ToList(); // Get the list of active territory IDs

                    //var result =
                    //      from ut in unitOfWork.Repository<UserTerritory>().GetAll().Where(x => x.IsActive)
                    //      join tr in unitOfWork.Repository<Territory>().GetAll().Where(x => x.IsActive) on ut.TerritoryId equals tr.Id
                    //      join usr in unitOfWork.Repository<AspNetUsers>().GetAll().Where(x => x.IsActive) on ut.UserId equals usr.Id
                    //      join url in unitOfWork.Repository<AspNetUserRoles>().GetAll() on usr.Id equals url.UserId
                    //      join dsf in unitOfWork.Repository<DSFRoute>().GetAll().Where(x => x.IsActive) on ut.UserId equals dsf.DSFId
                    //      join rt in unitOfWork.Repository<Route>().GetAll().Where(x => x.IsActive && x.VisitDay == todayVisitDay) on dsf.RouteId equals rt.Id
                    //      join rol in unitOfWork.Repository<AspNetRoles>().GetAll().Where(x => x.IsActive && x.Name == "DSF") on url.RoleId equals rol.Id
                    //      join att in unitOfWork.Repository<Attachments>().GetAll().Where(x => x.IsActive) on usr.Id equals att.UserId into attGroup
                    //      from att in attGroup.DefaultIfEmpty() // Left join for Attachments
                    //      join rts in unitOfWork.Repository<RouteShop>().GetAll() on rt.Id equals rts.RouteId
                    //      join msv in unitOfWork.Repository<MarkShopVisit>().GetAll().Where(x => x.IsActive == true && Convert.ToDateTime(x.CreatedDate).Date == DateTime.Now.Date) on rts.ShopId equals msv.ShopId into msvGroup
                    //      from msv in msvGroup.DefaultIfEmpty() // Left join for MarkShopVisits
                    //      where activeTerritoryIds.Contains(ut.TerritoryId) // Use Contains instead of IN
                    //      where usr.IsActive && rol.IsActive && tr.IsActive && rt.IsActive
                    //      select new
                    //      {
                    //          TerritoryId = tr.Id,
                    //          TerritoryName = tr.Name,
                    //          RouteId = rt.Id,
                    //          RouteName = rt.Name,
                    //          DSFName = usr.FirstName + " " + usr.LastName,
                    //          RoleName = rol.Name,
                    //          UserImage = att.ImageName,
                    //          VisitedShop = msv != null ? 1 : 0 // Case condition for visited shop
                    //      };

                    //// Grouping the results to count the number of visited shops for each route
                    //var groupedResult = result
                    //    .GroupBy(x => new
                    //    {
                    //        x.TerritoryId,
                    //        x.TerritoryName,
                    //        x.RouteId,
                    //        x.RouteName,
                    //        x.DSFName,
                    //        x.RoleName,
                    //        x.UserImage
                    //    })
                    //    .Select(g => new
                    //    {
                    //        g.Key.TerritoryId,
                    //        g.Key.TerritoryName,
                    //        g.Key.RouteId,
                    //        g.Key.RouteName,
                    //        g.Key.DSFName,
                    //        g.Key.RoleName,
                    //        g.Key.UserImage,
                    //        VisitedShop = g.Sum(x => x.VisitedShop) // Sum of visited shops in the group
                    //    }).ToList();

                    var result =
                        from ut in unitOfWork.Repository<UserTerritory>().GetAll().Where(x => x.IsActive)
                        join tr in unitOfWork.Repository<Territory>().GetAll().Where(x => x.IsActive) on ut.TerritoryId equals tr.Id
                        join usr in unitOfWork.Repository<AspNetUsers>().GetAll().Where(x => x.IsActive) on ut.UserId equals usr.Id
                        join url in unitOfWork.Repository<AspNetUserRoles>().GetAll() on usr.Id equals url.UserId
                        join rol in unitOfWork.Repository<AspNetRoles>().GetAll()
                            .Where(x => x.IsActive && x.Name == "DSF") on url.RoleId equals rol.Id
                        join dsf in unitOfWork.Repository<DSFRoute>().GetAll().Where(x => x.IsActive) on ut.UserId equals dsf.DSFId
                        join rt in unitOfWork.Repository<Route>().GetAll()
                            .Where(x => x.IsActive
                            //&& x.VisitDay == todayVisitDay
                            ) on dsf.RouteId equals rt.Id
                        join rts in unitOfWork.Repository<RouteShop>().GetAll().Where(x => x.IsActive) on rt.Id equals rts.RouteId
                        join sp in unitOfWork.Repository<Shop>().GetAll().Where(x => x.IsActive) on rts.ShopId equals sp.Id
                        join att in unitOfWork.Repository<Attachments>().GetAll().Where(x => x.IsActive) on usr.Id equals att.UserId into attGroup
                        from att in attGroup.DefaultIfEmpty() // Left join for Attachments
                        join msv in unitOfWork.Repository<MarkShopVisit>().GetAll().Where(x => x.IsActive) on rts.ShopId equals msv.ShopId into msvGroup
                        from msv in msvGroup.DefaultIfEmpty() // Left join for MarkShopVisits
                        where activeTerritoryIds.Contains(ut.TerritoryId) // Filters for active territories
                        group new { rts, msv } by new
                        {
                            TerritoryId = tr.Id,
                            TerritoryName = tr.Name,
                            RouteId = rt.Id,
                            RouteName = rt.Name,
                            DSFName = usr.FirstName + " " + usr.LastName,
                            RoleName = rol.Name,
                            UserImage = att.ImageName
                        } into grouped
                        select new
                        {
                            TerritoryId = grouped.Key.TerritoryId,
                            TerritoryName = grouped.Key.TerritoryName,
                            RouteId = grouped.Key.RouteId,
                            RouteName = grouped.Key.RouteName,
                            DSFName = grouped.Key.DSFName,
                            RoleName = grouped.Key.RoleName,
                            UserImage = !string.IsNullOrEmpty(grouped.Key.UserImage)
                            && System.IO.File.Exists(Path.Combine(Localcontainer, grouped.Key.UserImage.TrimStart('/')))
                            ? GetImageAsBase64(Path.Combine(Localcontainer, grouped.Key.UserImage.TrimStart('/')))
                            : null,
                            VisitedShop = grouped.Sum(x => x.msv != null ? 1 : 0) // Sum for visited shops
                        };




                    //var shopListResult = result.ToList();                    
                    var getTodayMyTeamStatusByUserIdresult = result.ToList();

                    return this.Result(ResponseStatus.OK, getTodayMyTeamStatusByUserIdresult, getTodayMyTeamStatusByUserIdresult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }


        [HttpGet]
        [Route("GetTodayRouteNotVisitedShopCountBySupervisorId")]
        public async Task<ActionResult<string>> GetTodayRouteNotVisitedShopCountBySupervisorId([FromQuery] string userId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var today = DateTime.Today;

                    var todayVisitDay = DateTime.Now.ToString("dddd");

                    var activeTerritoryIds = unitOfWork.Repository<UserTerritory>().GetAll()
                        .Where(subUt => subUt.UserId == new Guid(userId) && subUt.IsActive)
                        .Select(subUt => subUt.TerritoryId)
                        .Distinct()
                        .ToList(); // Get the list of active territory IDs


                    var result =
                        from ut in unitOfWork.Repository<UserTerritory>().GetAll().Where(x => x.IsActive)
                        join tr in unitOfWork.Repository<Territory>().GetAll().Where(x => x.IsActive) on ut.TerritoryId equals tr.Id
                        join usr in unitOfWork.Repository<AspNetUsers>().GetAll().Where(x => x.IsActive) on ut.UserId equals usr.Id
                        join url in unitOfWork.Repository<AspNetUserRoles>().GetAll() on usr.Id equals url.UserId
                        join rol in unitOfWork.Repository<AspNetRoles>().GetAll()
                            .Where(x => x.IsActive && x.Name == "DSF") on url.RoleId equals rol.Id
                        join dsf in unitOfWork.Repository<DSFRoute>().GetAll().Where(x => x.IsActive) on ut.UserId equals dsf.DSFId
                        join rt in unitOfWork.Repository<Route>().GetAll()
                            .Where(x => x.IsActive
                            //&& x.VisitDay == todayVisitDay
                            ) on dsf.RouteId equals rt.Id
                        join rts in unitOfWork.Repository<RouteShop>().GetAll().Where(x => x.IsActive) on rt.Id equals rts.RouteId
                        join sp in unitOfWork.Repository<Shop>().GetAll().Where(x => x.IsActive) on rts.ShopId equals sp.Id
                        join att in unitOfWork.Repository<Attachments>().GetAll().Where(x => x.IsActive) on usr.Id equals att.UserId into attGroup
                        from att in attGroup.DefaultIfEmpty() // Left join for Attachments
                        join msv in unitOfWork.Repository<MarkShopVisit>().GetAll().Where(x => x.IsActive) on rts.ShopId equals msv.ShopId into msvGroup
                        from msv in msvGroup.DefaultIfEmpty() // Left join for MarkShopVisits
                        where activeTerritoryIds.Contains(ut.TerritoryId) // Filters for active territories
                        group new { rts, msv } by new
                        {
                            TerritoryId = tr.Id,
                            TerritoryName = tr.Name,
                            RouteId = rt.Id,
                            RouteName = rt.Name,
                            DSFName = usr.FirstName + " " + usr.LastName,
                            RoleName = rol.Name,
                            UserImage = att.ImageName
                        } into grouped
                        select new
                        {
                            TerritoryId = grouped.Key.TerritoryId,
                            TerritoryName = grouped.Key.TerritoryName,
                            RouteId = grouped.Key.RouteId,
                            RouteName = grouped.Key.RouteName,
                            DSFName = grouped.Key.DSFName,
                            RoleName = grouped.Key.RoleName,
                            UserImage = !string.IsNullOrEmpty(grouped.Key.UserImage)
                            && System.IO.File.Exists(Path.Combine(Localcontainer, grouped.Key.UserImage.TrimStart('/')))
                            ? GetImageAsBase64(Path.Combine(Localcontainer, grouped.Key.UserImage.TrimStart('/')))
                            : null,
                            NotVisitedShop = grouped.Sum(x => x.msv == null ? 1 : 0) // Sum for Not visited shops
                        };

                    //var shopListResult = result.ToList();                    
                    var getTodayMyTeamStatusByUserIdresult = result.ToList();

                    return this.Result(ResponseStatus.OK, getTodayMyTeamStatusByUserIdresult, getTodayMyTeamStatusByUserIdresult.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }
        #endregion

        #region Dealership Order

        [HttpGet]
        [Route("GetProductForDOBySupId")]
        public async Task<ActionResult<string>> GetProductForDOBySupId([FromQuery] string userId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "User Id Is Compulsory");
                    }

                    var products = (from item in unitOfWork.Repository<Entities.Models.Item>().GetAll()
                                    join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll()
                                        on item.ItemTypeId equals itemType.Id
                                    join subCategory in unitOfWork.Repository<Entities.Models.SubCategory>().GetAll()
                                        on itemType.SubCategoryId equals subCategory.Id
                                    join category in unitOfWork.Repository<Entities.Models.Category>().GetAll()
                                        on subCategory.CategoryId equals category.Id
                                    join categoryStore in unitOfWork.Repository<Entities.Models.CategoryStore>().GetAll()
                                        on category.Id equals categoryStore.CategoryId
                                    join store in unitOfWork.Repository<Entities.Models.Store>().GetAll()
                                        on categoryStore.StoreId equals store.Id
                                    where store.Id == 3 && category.CompanyId == lIntKhilafatCompanyId
                                    orderby item.Name
                                    select new
                                    {
                                        item.Id,
                                        item.Name,
                                        Type = itemType.Name,
                                        VolumeInMl = item.Volume,
                                        item.QuantityInPack,
                                        Image = !string.IsNullOrEmpty(item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                               ? GetImageAsBase64(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                               : null
                                    }).ToList();


                    var getDealershipResult = (from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                                               where ut.UserId == new Guid(userId) && ut.IsActive
                                               join ds in unitOfWork.Repository<Dealership>().GetAll() on ut.TerritoryId equals ds.TerritoryId
                                               where ds.DealershipTypeId == 1
                                               select new DealershipProductVM
                                               {
                                                   DealershipId = ds.Id,
                                                   DealershipName = ds.Name,
                                                   PhoneNo = ds.PhoneNo,
                                                   Address = ds.Address,
                                                   PinLocation = ds.PinLocation,
                                                   Products = (from item in products
                                                               join pgd in unitOfWork.Repository<PriceGroupDetails>().GetAll() on item.Id equals pgd.ItemId
                                                               join dpg in unitOfWork.Repository<DistributorPriceGroup>().GetAll() on pgd.PriceGroupId equals dpg.PriceGroupId
                                                               where pgd.IsActive && dpg.IsActive && dpg.DealershipId == ds.Id
                                                               select new ProductResult
                                                               {
                                                                   ProductId = item.Id,
                                                                   Name = item.Name,
                                                                   Type = item.Type,
                                                                   VolumeInMl = item.VolumeInMl,
                                                                   DistributorPrice = pgd.NetDistributorPrice,
                                                                   QuantityInPack = item.QuantityInPack,
                                                                   ImageName = item.Image
                                                               }).ToList()
                                               }).Distinct().ToList();

                    if (getDealershipResult == null || !getDealershipResult.Any())
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Dealership not mapped");
                    }

                    var firstDealershipWithProducts = getDealershipResult.FirstOrDefault();

                    return this.Result(ResponseStatus.OK, firstDealershipWithProducts, "Dealership Order Product");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("GetProductForDOByDistId")]
        public async Task<ActionResult<string>> GetProductForDOBySupIdDistId([FromQuery] long dealershipId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (dealershipId == 0)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "dist Id Is Compulsory");
                    }


                    var distributorPriceGroups = await unitOfWork.Repository<Entities.Models.DistributorPriceGroup>()
       .GetAsync(x => x.DealershipId == dealershipId && x.IsActive == true && x.IsDelete == false);

                    if (distributorPriceGroups == null || !distributorPriceGroups.Any())
                    {
                        return this.Result(ResponseStatus.ValidationFailed, null, "No active Distributor Price Groups found for the Selected Distributor");
                    }

                    var priceGroupIds = distributorPriceGroups.Select(x => x.PriceGroupId).ToList();

                    var priceDetails = await unitOfWork.Repository<PriceGroupDetails>()
                        .GetAsync(x => priceGroupIds.Contains(x.PriceGroupId) && x.IsActive == true && x.IsDelete == false);

                    var getDealershipResult = (from ds in unitOfWork.Repository<Dealership>().GetAll()
                                               where ds.DealershipTypeId == 1 && ds.Id == dealershipId
                                               select new DealershipProductVM
                                               {
                                                   DealershipId = ds.Id,
                                                   DealershipName = ds.Name,
                                                   PhoneNo = ds.PhoneNo,
                                                   Address = ds.Address,
                                                   PinLocation = ds.PinLocation,
                                                   Products = (from item in unitOfWork.Repository<Entities.Models.Item>().GetAll()
                                                               join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll()
                                                                   on item.ItemTypeId equals itemType.Id
                                                               join subCategory in unitOfWork.Repository<Entities.Models.SubCategory>().GetAll()
                                                                   on itemType.SubCategoryId equals subCategory.Id
                                                               join category in unitOfWork.Repository<Entities.Models.Category>().GetAll()
                                                                   on subCategory.CategoryId equals category.Id
                                                               join categoryStore in unitOfWork.Repository<Entities.Models.CategoryStore>().GetAll()
                                                                   on category.Id equals categoryStore.CategoryId
                                                               join store in unitOfWork.Repository<Entities.Models.Store>().GetAll()
                                                                   on categoryStore.StoreId equals store.Id
                                                               join pgd in unitOfWork.Repository<PriceGroupDetails>().GetAll()
                                                                   on item.Id equals pgd.ItemId into priceGroup
                                                               from pgd in priceGroup.DefaultIfEmpty()
                                                               join dpg in unitOfWork.Repository<DistributorPriceGroup>().GetAll()
                                                                    on (pgd != null ? pgd.PriceGroupId : (long?)null) equals dpg.PriceGroupId into distributorGroup
                                                               from dpg in distributorGroup.DefaultIfEmpty()
                                                               where store.Id == 3 && category.CompanyId == lIntKhilafatCompanyId
                                                                     && (pgd == null || (priceGroupIds.Contains(pgd.PriceGroupId) && pgd.IsActive == true && pgd.IsDelete == false))
                                                                     && (dpg == null || (dpg.DealershipId == dealershipId && dpg.IsActive == true && dpg.IsDelete == false))
                                                               orderby item.Name
                                                               select new ProductResult
                                                               {
                                                                   ProductId = item.Id,
                                                                   Name = item.Name,
                                                                   Type = itemType.Name,
                                                                   VolumeInMl = item.Volume,
                                                                   DistributorPrice = pgd != null ? pgd.NetDistributorPrice : 0,
                                                                   QuantityInPack = item.QuantityInPack,
                                                                   ImageName = !string.IsNullOrEmpty(item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                                                               ? GetImageAsBase64(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                                                               : null,


                                                               }).Distinct().ToList()
                                               }).Distinct().ToList();


                    //var getDealershipResult = (from ds in unitOfWork.Repository<Dealership>().GetAll()
                    //                           where ds.Id == dealershipId
                    //                           select new DealershipProductVM
                    //                           {
                    //                               DealershipId = ds.Id,
                    //                               DealershipName = ds.Name,
                    //                               PhoneNo = ds.PhoneNo,
                    //                               Address = ds.Address,
                    //                               PinLocation = ds.PinLocation,
                    //                               Products = (from prd in unitOfWork.Repository<Product>().GetAll()
                    //                                           join att in unitOfWork.Repository<Attachments>().GetAll()
                    //                                               on prd.Id equals att.ProductId
                    //                                           join price in unitOfWork.Repository<PriceGroupDetails>().GetAll()
                    //                                               on prd.Id equals price.ProductId into priceGroup
                    //                                           from price in priceGroup.DefaultIfEmpty()
                    //                                           where prd.IsActive == true
                    //                                                 && att.IsActive == true
                    //                                                 && (price == null || (priceGroupIds.Contains(price.PriceGroupId) && price.IsActive == true && price.IsDelete == false))
                    //                                           select new ProductResult
                    //                                           {
                    //                                               ProductId = prd.Id,
                    //                                               Name = prd.Name,
                    //                                               Type = prd.Type,
                    //                                               VolumeInMl = prd.VolumeInMl,
                    //                                               DistributorPrice = price != null ? price.NetDistributorPrice : prd.DistributorPrice, // Use price if available
                    //                                               QuantityInPack = prd.QuantityInPack,
                    //                                               ImageName = att.ImageName
                    //                                           }).ToList()
                    //                           }).Distinct().ToList();



                    if (getDealershipResult == null || !getDealershipResult.Any())
                    {
                        return this.Result(ResponseStatus.BadRequest, null, "Dealership not mapped");
                    }

                    var firstDealershipWithProducts = getDealershipResult.FirstOrDefault();

                    return this.Result(ResponseStatus.OK, firstDealershipWithProducts, "Dealership Order Product");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("SaveDealershipOrder")]
        public async Task<ActionResult<string>> SaveDealershipOrder(SaveDealershipOrderCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }

                    Entities.Models.Order _order = new Entities.Models.Order();
                    _order.DealershipId = command.DealershipId;
                    _order.OrderStatusId = (long)OrderStatusEnum.OrderCreate;

                    _order.CreatedById = new Guid(command.UserId);
                    _order.CreatedDate = DateTime.Now;
                    _order.DealershipAddress = command.Address;
                    unitOfWork.Repository<Entities.Models.Order>().Add(_order);
                    unitOfWork.SaveChanges();

                    var distributorPriceGroups = await unitOfWork.Repository<Entities.Models.DistributorPriceGroup>().GetAsync(x => x.DealershipId == command.DealershipId && x.IsActive == true && x.IsDelete == false);


                    foreach (var item in command.OrderItemCommandList)
                    {
                        if (item.OrderQuantity > 0)
                        {
                            var itemPriceGroupDetails = await unitOfWork.Repository<Entities.Models.PriceGroupDetails>().GetAsync(x => x.PriceGroupId == distributorPriceGroups.FirstOrDefault().PriceGroupId && x.ItemId == item.ProductId && x.IsActive == true && x.IsDelete == false);

                            var lstitemPriceGroupDetails = itemPriceGroupDetails.ToList().FirstOrDefault();

                            Entities.Models.OrderItems _orderItems = new Entities.Models.OrderItems();
                            _orderItems.IsActive = true;
                            _orderItems.IsDelete = false;
                            _orderItems.OrderId = _order.Id;
                            _orderItems.ItemId = item.ProductId;
                            _orderItems.Quantity = item.OrderQuantity;
                            _orderItems.CreatedById = new Guid(command.UserId);
                            _orderItems.CreatedDate = DateTime.Now;

                            _orderItems.DistributorPrice = item.DistributorPrice;
                            _orderItems.DistributorPromo = lstitemPriceGroupDetails.DistributorPromo;
                            _orderItems.TradePrice = lstitemPriceGroupDetails.TradePrice;
                            _orderItems.RetailPrice = lstitemPriceGroupDetails.RetailPrice;

                            unitOfWork.Repository<Entities.Models.OrderItems>().Add(_orderItems);
                            unitOfWork.SaveChanges();
                        }
                    }

                    OrderProcess process = new OrderProcess();
                    process.OrderId = _order.Id;
                    process.FromStatusId = null;
                    process.ToStatusId = _order.OrderStatusId;
                    process.Comments = "New Order Created";
                    process.CreatedById = new Guid(command.UserId);
                    process.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<OrderProcess>().Add(process);
                    unitOfWork.SaveChanges();

                    if(!string.IsNullOrEmpty(command.ImageFileSource))
                    {
                        Attachments attachment = new()
                        {
                            CreatedDate = DateTime.Now,
                            CreatedById = new Guid(command.UserId),
                            OrderId = _order.Id
                        };

                        BlobImageUploadModel blobModel = new()
                        {
                            File = "data:image/jpeg;base64," + command.ImageFileSource,
                            FileName = command.ImageExtension,
                            FolderName = "assets/Files/Order"
                        };

                        attachment.ImageName = "/assets/Files/Order/" + await blobService.UploadBase64FileToBlobAsync(blobModel, command.ImageExtension);
                        await unitOfWork.Repository<Attachments>().AddAsync(attachment);
                        unitOfWork.SaveChanges();
                    }

                    return this.Result(ResponseStatus.OK, _order.Id, "Order Placed Successfully");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("GetDealerOrderBySupId")]
        public async Task<ActionResult<string>> GetDealerOrderBySupId([FromQuery] string userId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "User Id Is Compulsory");
                    }

                    //var query = from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                    //            join ter in unitOfWork.Repository<Territory>().GetAll() on ut.TerritoryId equals ter.Id
                    //            join dlr in unitOfWork.Repository<Dealership>().GetAll() on ut.TerritoryId equals dlr.TerritoryId
                    //            join ord in unitOfWork.Repository<Order>().GetAll() on dlr.Id equals ord.DealershipId
                    //            join oi in unitOfWork.Repository<OrderItems>().GetAll() on ord.Id equals oi.OrderId
                    //            join attOrder in unitOfWork.Repository<Attachments>().GetAll() on ord.Id equals attOrder.OrderId
                    //            join prd in unitOfWork.Repository<Product>().GetAll() on oi.ProductId equals prd.Id
                    //            join att in unitOfWork.Repository<Attachments>().GetAll() on prd.Id equals att.ProductId
                    //            join os in unitOfWork.Repository<Status>().GetAll() on ord.OrderStatusId equals os.Id
                    //            where ord.IsActive == true
                    //                  && oi.IsActive == true
                    //                  && att.IsActive == true
                    //                  && attOrder.IsActive == true
                    //                  && ut.UserId == new Guid(userId)
                    //            group new
                    //            {
                    //                ProductId = prd.Id,
                    //                ProductName = prd.Name,
                    //                ProductType = prd.Type,
                    //                VolumeInMl = prd.VolumeInMl,
                    //                Quantity = oi.Quantity,
                    //                DistributorPrice = oi.DistributorPrice,
                    //                ImageName = att.ImageName
                    //            } by new
                    //            {
                    //                ord.Id,
                    //                TerritoryName = ter.Name,
                    //                DealerShipName = dlr.Name,
                    //                OrderDate = ord.CreatedDate,
                    //                Address = ord.DealershipAddress,
                    //                OrderStatus = os.Name,
                    //                OrderImage = attOrder.ImageName,
                    //            } into orderGroup
                    //            orderby orderGroup.Key.Id descending // Order by OrderID descending
                    //            select new
                    //            {
                    //                TerritoryName = orderGroup.Key.TerritoryName,
                    //                DealerShipName = orderGroup.Key.DealerShipName,
                    //                OrderDate = orderGroup.Key.OrderDate,
                    //                Address = orderGroup.Key.Address,
                    //                OrderID = orderGroup.Key.Id,
                    //                OrderStatus = orderGroup.Key.OrderStatus,
                    //                OrderImage = orderGroup.Key.OrderImage,
                    //                Products = orderGroup
                    //                    .Select(p => new
                    //                    {
                    //                        p.ProductId,
                    //                        p.ProductName,
                    //                        p.ProductType,
                    //                        p.VolumeInMl,
                    //                        p.Quantity,
                    //                        p.DistributorPrice,
                    //                        p.ImageName
                    //                    })
                    //                    .Distinct()
                    //                    .ToList()
                    //            };

                    var query = from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                                join ter in unitOfWork.Repository<Territory>().GetAll() on ut.TerritoryId equals ter.Id
                                join dlr in unitOfWork.Repository<Dealership>().GetAll() on ut.TerritoryId equals dlr.TerritoryId
                                join ord in unitOfWork.Repository<Entities.Models.Order>().GetAll() on dlr.Id equals ord.DealershipId
                                join oi in unitOfWork.Repository<OrderItems>().GetAll() on ord.Id equals oi.OrderId
                                join attOrder in unitOfWork.Repository<Attachments>().GetAll() on ord.Id equals attOrder.OrderId
                                join item in unitOfWork.Repository<Entities.Models.Item>().GetAll() on oi.ItemId equals item.Id
                                join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll() on item.ItemTypeId equals itemType.Id
                                join os in unitOfWork.Repository<Status>().GetAll() on ord.OrderStatusId equals os.Id
                                where dlr.DealershipTypeId == 1 && ord.IsActive == true
                                      && oi.IsActive == true
                                      && attOrder.IsActive == true
                                      && ut.UserId == new Guid(userId)
                                group new
                                {
                                    ProductId = item.Id,
                                    ProductName = item.Name,
                                    ProductType = itemType.Name,
                                    VolumeInMl = item.Volume,
                                    Quantity = oi.Quantity,
                                    DistributorPrice = oi.DistributorPrice,
                                    ImageName = item.Image
                                } by new
                                {
                                    ord.Id,
                                    TerritoryName = ter.Name,
                                    DealerShipName = dlr.Name,
                                    OrderDate = ord.CreatedDate,
                                    Address = ord.DealershipAddress,
                                    OrderStatus = os.Title,
                                    OrderImage = attOrder.ImageName,
                                } into orderGroup
                                orderby orderGroup.Key.Id descending // Order by OrderID descending
                                select new
                                {
                                    TerritoryName = orderGroup.Key.TerritoryName,
                                    DealerShipName = orderGroup.Key.DealerShipName,
                                    OrderDate = orderGroup.Key.OrderDate,
                                    Address = orderGroup.Key.Address,
                                    OrderID = orderGroup.Key.Id,
                                    OrderStatus = orderGroup.Key.OrderStatus,
                                    OrderImage = !string.IsNullOrEmpty(orderGroup.Key.OrderImage) && System.IO.File.Exists(Path.Combine(Localcontainer, orderGroup.Key.OrderImage.TrimStart('/')))
                                                 ? GetImageAsBase64(Path.Combine(Localcontainer, orderGroup.Key.OrderImage.TrimStart('/')))
                                                 : null,
                                    Products = orderGroup
                                        .Select(p => new
                                        {
                                            p.ProductId,
                                            p.ProductName,
                                            p.ProductType,
                                            p.VolumeInMl,
                                            p.Quantity,
                                            p.DistributorPrice,
                                            ImageName = !string.IsNullOrEmpty(p.ImageName) && System.IO.File.Exists(Path.Combine(Localcontainer, p.ImageName.TrimStart('/')))
                                                           ? GetImageAsBase64(Path.Combine(Localcontainer, p.ImageName.TrimStart('/')))
                                                           : null
                                        })
                                        .Distinct()
                                        .ToList()
                                };

                    var result = query.ToList();


                    return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("GetDealerOrderStatusWiseBySupId")]
        public async Task<ActionResult<string>> GetDealerOrderStatusWiseBySupId([FromQuery] string userId, [FromQuery] int statusId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "User Id Is Compulsory");
                    }

                    var activeDistributor = (
                            from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                            join ar in unitOfWork.Repository<Area>().GetAll() on ut.AreaId equals ar.Id
                            join tr in unitOfWork.Repository<Territory>().GetAll() on ar.Id equals tr.AreaId
                            join dis in unitOfWork.Repository<Dealership>().GetAll() on tr.Id equals dis.TerritoryId
                            where dis.DealershipTypeId == 1 && ut.UserId == Guid.Parse(userId) &&
                                  ut.IsActive && !ut.IsDelete &&
                                  ar.IsActive && !ar.IsDelete &&
                                  tr.IsActive && !tr.IsDelete &&
                                  dis.IsActive && !dis.IsDelete
                            select new { Name = "ID", Value = dis.Id }
                        ).ToList();

                    if (statusId <= (long)OrderStatusEnum.OrderConfirm && statusId > (long)OrderStatusEnum.OrderCanceled)
                    {
                        var query = from dlr in unitOfWork.Repository<Dealership>().GetAll()
                                    join ord in unitOfWork.Repository<Entities.Models.Order>().GetAll() on dlr.Id equals ord.DealershipId
                                    join oi in unitOfWork.Repository<OrderItems>().GetAll() on ord.Id equals oi.OrderId
                                    join attOrder in unitOfWork.Repository<Attachments>().GetAll() on ord.Id equals attOrder.OrderId
                                    join item in unitOfWork.Repository<Entities.Models.Item>().GetAll() on oi.ItemId equals item.Id
                                    join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll() on item.ItemTypeId equals itemType.Id
                                    join os in unitOfWork.Repository<Status>().GetAll() on ord.OrderStatusId equals os.Id
                                               into osJoin
                                    from os in (statusId > 0 ? osJoin.Where(o => o.Id == statusId) : osJoin.DefaultIfEmpty())
                                    join dod in unitOfWork.Repository<Entities.Models.DispatchOrder>().GetAll() on ord.Id equals dod.OrderId
                                    into dodJoin
                                    from dod in dodJoin.DefaultIfEmpty()
                                    where dlr.DealershipTypeId == 1 && ord.IsActive == true
                                          && oi.IsActive == true
                                          && attOrder.IsActive == true
                                         && activeDistributor.Any(ad => ad.Value == dlr.Id)
                                    group new
                                    {
                                        ProductId = item.Id,
                                        ProductName = item.Name,
                                        ProductType = itemType.Name,
                                        VolumeInMl = item.Volume,
                                        Quantity = oi.Quantity,
                                        DistributorPrice = oi.DistributorPrice,
                                        ImageName = item.Image
                                    } by new
                                    {
                                        ord.Id,
                                        DealerShipName = dlr.Name,
                                        OrderDate = ord.CreatedDate,
                                        Address = ord.DealershipAddress,
                                        OrderStatus = os.Title,
                                        OrderImage = attOrder.ImageName,
                                        DeliveryDateTime = dod != null ? dod.CreatedDate : null,
                                        DeliveryChallanCode = dod != null ? dod.DCCode : null,
                                    } into orderGroup
                                    orderby orderGroup.Key.Id descending // Order by OrderID descending
                                    select new
                                    {
                                        DealerShipName = orderGroup.Key.DealerShipName,
                                        OrderDate = orderGroup.Key.OrderDate,
                                        Address = orderGroup.Key.Address,
                                        OrderID = orderGroup.Key.Id,
                                        OrderStatus = orderGroup.Key.OrderStatus,
                                        OrderImage = !string.IsNullOrEmpty(orderGroup.Key.OrderImage) && System.IO.File.Exists(Path.Combine(Localcontainer, orderGroup.Key.OrderImage.TrimStart('/')))
                                                     ? GetImageAsBase64(Path.Combine(Localcontainer, orderGroup.Key.OrderImage.TrimStart('/')))
                                                     : null,
                                        DeliveryDateTime = orderGroup.Key.DeliveryDateTime,
                                        DeliveryChallanCode = orderGroup.Key.DeliveryChallanCode,
                                        Products = orderGroup
                                            .Select(p => new
                                            {
                                                p.ProductId,
                                                p.ProductName,
                                                p.ProductType,
                                                p.VolumeInMl,
                                                p.Quantity,
                                                p.DistributorPrice,
                                                ImageName = !string.IsNullOrEmpty(p.ImageName) && System.IO.File.Exists(Path.Combine(Localcontainer, p.ImageName.TrimStart('/')))
                                                           ? GetImageAsBase64(Path.Combine(Localcontainer, p.ImageName.TrimStart('/')))
                                                           : null
                                            })
                                            .Distinct()
                                            .ToList()
                                    };

                        var result = query.ToList();


                        return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                    }
                    else
                    {
                        var query = from dlr in unitOfWork.Repository<Dealership>().GetAll()
                                    join ord in unitOfWork.Repository<Entities.Models.Order>().GetAll() on dlr.Id equals ord.DealershipId
                                    join attOrder in unitOfWork.Repository<Attachments>().GetAll() on ord.Id equals attOrder.OrderId
                                    join os in unitOfWork.Repository<Status>().GetAll() on ord.OrderStatusId equals os.Id
                                    where dlr.DealershipTypeId == 1 && ord.IsActive == true
                                          && attOrder.IsActive == true
                                          && activeDistributor.Any(ad => ad.Value == dlr.Id)
                                           && os.Id == statusId
                                    orderby ord.Id descending
                                    select new
                                    {
                                        OrderID = ord.Id,
                                        DealerShipName = dlr.Name,
                                        OrderDate = ord.CreatedDate,
                                        Address = ord.DealershipAddress,
                                        OrderStatus = os.Title,
                                        OrderImage = !string.IsNullOrEmpty(attOrder.ImageName) && System.IO.File.Exists(Path.Combine(Localcontainer, attOrder.ImageName.TrimStart('/')))
                                                     ? GetImageAsBase64(Path.Combine(Localcontainer, attOrder.ImageName.TrimStart('/')))
                                                     : null,
                                        OrderProductsGroup = (from oi in unitOfWork.Repository<OrderItems>().GetAll()
                                                              join item in unitOfWork.Repository<Entities.Models.Item>().GetAll() on oi.ItemId equals item.Id
                                                              join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll() on item.ItemTypeId equals itemType.Id
                                                              where oi.OrderId == ord.Id && oi.IsActive == true
                                                              group new { oi, item, itemType } by new
                                                              {
                                                                  ItemId = item.Id,
                                                                  ItemName = item.Name,
                                                                  ItemType = itemType.Name,
                                                                  ItemVolume = item.Volume,
                                                                  ItemImage = item.Image,
                                                                  ItemDistributorPrice = oi.DistributorPrice
                                                              } into productGroup
                                                              select new
                                                              {
                                                                  ProductId = productGroup.Key.ItemId,
                                                                  ProductName = productGroup.Key.ItemName,
                                                                  ProductType = productGroup.Key.ItemType,
                                                                  VolumeInMl = productGroup.Key.ItemVolume,
                                                                  TotalQuantity = productGroup.Sum(p => p.oi.Quantity), // Sum quantity
                                                                  DistributorPrice = productGroup.Key.ItemDistributorPrice,
                                                                  ImageName = !string.IsNullOrEmpty(productGroup.Key.ItemImage) && System.IO.File.Exists(Path.Combine(Localcontainer, productGroup.Key.ItemImage.TrimStart('/')))
                                                                            ? GetImageAsBase64(Path.Combine(Localcontainer, productGroup.Key.ItemImage.TrimStart('/')))
                                                                            : null
                                                              }).ToList(),

                                        Dispatches = (from dis in unitOfWork.Repository<Dispatch>().GetAll()
                                                      join vhc in unitOfWork.Repository<Vehicle>().GetAll() on dis.VehicleId equals vhc.Id
                                                      join doo in unitOfWork.Repository<Entities.Models.DispatchOrder>().GetAll() on dis.Id equals doo.DispatchId
                                                      where dis.IsActive == true && dis.StatusId == (long)OrderStatusEnum.Approved && doo.OrderId == ord.Id
                                                      select new
                                                      {
                                                          DispatchDate = dis.ApprovedDate,
                                                          BiltyNo = dis.BiltyNo,
                                                          FreightCharges = dis.FreightCharges,
                                                          VehicleName = vhc.VehicleName,
                                                          RegistrationNumber = vhc.RegistrationNumber,
                                                          DriverName = vhc.DriverName,
                                                          DriverPhoneNo = vhc.DriverPhoneNo,
                                                          DispatchOrderId = doo.Id,
                                                          DCCode = doo.DCCode,
                                                          StatusId = doo.StatusId,
                                                          Items = (from dd in unitOfWork.Repository<DispatchDetail>().GetAll()
                                                                   join oi in unitOfWork.Repository<OrderItems>().GetAll() on dd.OrderItemId equals oi.Id
                                                                   join item in unitOfWork.Repository<Entities.Models.Item>().GetAll() on oi.ItemId equals item.Id
                                                                   join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll() on item.ItemTypeId equals itemType.Id
                                                                   where dd.DispatchOrderId == doo.Id
                                                                   select new
                                                                   {
                                                                       OrderItemId = dd.OrderItemId,
                                                                       Quantity = dd.Quantity,
                                                                       ProductId = item.Id,
                                                                       ProductName = item.Name,
                                                                       ProductType = itemType.Name,
                                                                       VolumeInMl = item.Volume,
                                                                       DistributorPrice = oi.DistributorPrice,
                                                                       ImageName = !string.IsNullOrEmpty(item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                                                                   ? GetImageAsBase64(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                                                                   : null
                                                                   }).ToList()
                                                      }).ToList()
                                    };

                        var result = query.ToList();

                        return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        #endregion

        #region ShopTaggingDetails

        [HttpGet]
        [Route("GetSupervisorDashboardByUserId")]
        public async Task<ActionResult<string>> GetSupervisorDashboardByUserId([FromQuery] string userId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "User Id Is Compulsory");
                    }
                    SupervisorDashboardVM LobjSupervisorDashboardVM = new SupervisorDashboardVM();
                    var today = DateTime.Today;
                    var visitDay = today.ToString("dddd");
                    // Step 1: Get the list of active territory IDs for the specific user.
                    var activeTerritoryIds = unitOfWork.Repository<UserTerritory>()
                        .GetAll()
                        .Where(subUt => subUt.UserId == new Guid(userId) && subUt.IsActive)
                        .Select(subUt => subUt.TerritoryId)
                        .Distinct()
                        .ToList();
                    // Step 2: Perform the main query
                    var result = from rt in unitOfWork.Repository<Route>().GetAll()
                                 join rts in unitOfWork.Repository<RouteShop>().GetAll() on rt.Id equals rts.RouteId
                                 join sp in unitOfWork.Repository<Shop>().GetAll() on rts.ShopId equals sp.Id
                                 join msv in unitOfWork.Repository<MarkShopVisit>().GetAll()
                                           .Where(msv => msv.IsActive && msv.CreatedDate.Value.Date == DateTime.Now.Date)
                                           on rts.ShopId equals msv.ShopId into msvJoin
                                 from msv in msvJoin.DefaultIfEmpty() // LEFT JOIN
                                 where
                                        activeTerritoryIds.Contains(rt.TerritoryId)
                                       && rt.IsActive
                                       && rts.IsActive
                                       && sp.IsActive
                                 //&& rt.VisitDay == visitDay
                                 select new
                                 {
                                     rts.Id,
                                     Visited = msv == null ? 0 : 1 // Set Visited to 1 if shop was visited, otherwise 0
                                 };


                    // Additional aggregation logic if needed
                    LobjSupervisorDashboardVM.TodayRouteShopVisited = result.Where(x => x.Visited == 1).Count();
                    LobjSupervisorDashboardVM.TotalRouteShopVisit = result.Count();

                    //Logic for today territory order

                    var todayTerritoryOrderCount = (
                        from ord in unitOfWork.Repository<Entities.Models.Order>().GetAll().Where(o => o.IsActive)
                        join sp in unitOfWork.Repository<Shop>().GetAll().Where(s => s.IsActive)
                        on ord.ShopId equals sp.Id
                        where ord.CreatedDate?.Date == DateTime.Now.Date // Compare only the date part
                            && activeTerritoryIds.Contains(sp.TerritoryId)
                        select ord.Id
                    ).Count();
                    if (todayTerritoryOrderCount != null && todayTerritoryOrderCount > 0)
                    {
                        LobjSupervisorDashboardVM.TodayTerritoryOrder = todayTerritoryOrderCount;
                    }



                    var resultTeam = (from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                                      join usr in unitOfWork.Repository<AspNetUsers>().GetAll() on ut.UserId equals usr.Id
                                      join url in unitOfWork.Repository<AspNetUserRoles>().GetAll() on usr.Id equals url.UserId
                                      join uat in unitOfWork.Repository<UserAttendance>().GetAll()
                                          .Where(a => a.IsActive && a.AttendanceDate.Date == DateTime.Now.Date) on usr.Id equals uat.UserId into uatGroup
                                      from uat in uatGroup.DefaultIfEmpty()  // Using DefaultIfEmpty here
                                      where (from subUt in unitOfWork.Repository<UserTerritory>().GetAll()
                                             where subUt.UserId == new Guid(userId) && subUt.IsActive
                                             select subUt.TerritoryId).Distinct().Contains(ut.TerritoryId)
                                            && ut.IsActive
                                            && ut.UserId != new Guid(userId)
                                      select new
                                      {
                                          TotalTeam = ut.Id,
                                          TotalOffline = (uat == null) ? 1 : 0,
                                          TotalPresent = (uat != null && (bool)uat.IsPresent) ? 1 : 0,
                                          TotalAbsent = (uat != null && (bool)!uat.IsPresent) ? 1 : 0
                                      }).ToList();
                    LobjSupervisorDashboardVM.TotalTeamMember = resultTeam.Count();
                    LobjSupervisorDashboardVM.OfflineTeamMember = resultTeam.Sum(x => x.TotalOffline);
                    LobjSupervisorDashboardVM.PresentTeamMember = resultTeam.Sum(x => x.TotalPresent);
                    LobjSupervisorDashboardVM.AbsentTeamMember = resultTeam.Sum(x => x.TotalAbsent);
                    LobjSupervisorDashboardVM.TotalPendingShopTaggingRequest = (from sp in unitOfWork.Repository<Entities.Models.Shop>().GetAll()
                                                                                where sp.IsActive == true
                                                                                      && sp.IsVerified == false
                                                                                      && sp.VerifiedById == null
                                                                                      && (from ut in unitOfWork.Repository<Entities.Models.UserTerritory>().GetAll()
                                                                                          where ut.UserId == new Guid(userId)
                                                                                                && ut.IsActive == true
                                                                                          select ut.TerritoryId).Distinct().Contains(sp.TerritoryId)
                                                                                select sp).Count();
                    return this.Result(ResponseStatus.OK, LobjSupervisorDashboardVM, "Dashboard Data");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("GetTerritoryShopTaggingStatusBySupId")]
        public async Task<ActionResult<string>> GetTerritoryShopTaggingStatusBySupId([FromQuery] string userId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "User Id Is Compulsory");
                    }

                    var query = from sp in unitOfWork.Repository<Entities.Models.Shop>().GetAll()
                                join u in unitOfWork.Repository<Entities.Models.AspNetUsers>().GetAll() on sp.CreatedById equals u.Id
                                join t in unitOfWork.Repository<Entities.Models.Territory>().GetAll() on sp.TerritoryId equals t.Id
                                join att in unitOfWork.Repository<Entities.Models.Attachments>().GetAll() on sp.Id equals att.ShopId into attGroup
                                from att in attGroup.DefaultIfEmpty() // Left join equivalent
                                where sp.IsActive == true
                                && (from ut in unitOfWork.Repository<Entities.Models.UserTerritory>().GetAll()
                                    where ut.UserId == new Guid(userId) // UserId
                                          && ut.IsActive == true
                                    select ut.TerritoryId).Distinct().Contains(sp.TerritoryId)
                                group sp by new { u.Id, u.FirstName, u.LastName, t.Name } into grouped
                                select new
                                {
                                    CreatedById = grouped.Key.Id,
                                    CreatedByName = grouped.Key.FirstName + " " + grouped.Key.LastName,
                                    TerritoryName = grouped.Key.Name,
                                    ApprovedShop = grouped.Count(sp => sp.IsVerified == true && sp.VerifiedById != null),
                                    RejectedShop = grouped.Count(sp => sp.IsVerified == false && sp.VerifiedById != null),
                                    PendingVerification = grouped.Count(sp => sp.IsVerified == false && sp.VerifiedById == null),
                                    TotalShop = grouped.Count()
                                };



                    var result = query.ToList();


                    return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }


        #endregion

        #region DSF Order
        [HttpGet]
        [Route("GetActiveShopOrderProductByDsfId")]
        public async Task<ActionResult<string>> GetActiveShopOrderProductByDsfId([FromQuery] string userId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "User Id Is Compulsory");
                    }

                    //var query = from p in unitOfWork.Repository<Entities.Models.Product>().GetAll()
                    //            join att in unitOfWork.Repository<Entities.Models.Attachments>().GetAll() on p.Id equals att.ProductId
                    //            where p.IsActive == true && p.IsDelete == false && att.IsActive == true && att.IsDelete == false
                    //            select new
                    //            {
                    //                p.Id,
                    //                p.Name,
                    //                att.ImageName,
                    //                p.Type,
                    //                p.VolumeInMl,
                    //                p.CreatedById,
                    //                p.RetailPrice,
                    //                p.TradePrice,
                    //                p.QuantityInPack
                    //            };


                    var query = (from item in unitOfWork.Repository<Entities.Models.Item>().GetAll()
                                 join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll()
                                     on item.ItemTypeId equals itemType.Id
                                 join subCategory in unitOfWork.Repository<Entities.Models.SubCategory>().GetAll()
                                     on itemType.SubCategoryId equals subCategory.Id
                                 join category in unitOfWork.Repository<Entities.Models.Category>().GetAll()
                                     on subCategory.CategoryId equals category.Id
                                 join categoryStore in unitOfWork.Repository<Entities.Models.CategoryStore>().GetAll()
                                     on category.Id equals categoryStore.CategoryId
                                 join store in unitOfWork.Repository<Entities.Models.Store>().GetAll()
                                     on categoryStore.StoreId equals store.Id
                                 join pgd in unitOfWork.Repository<Entities.Models.PriceGroupDetails>().GetAll()
                                     on item.Id equals pgd.ItemId into priceGroup
                                 from pgd in priceGroup.DefaultIfEmpty()
                                 join dpg in unitOfWork.Repository<Entities.Models.DistributorPriceGroup>().GetAll()
                                     on pgd.PriceGroupId equals dpg.PriceGroupId into distributorPrice
                                 from dpg in distributorPrice.DefaultIfEmpty()
                                 where store.Id == 3
                                       && category.CompanyId == lIntKhilafatCompanyId
                                       && (pgd == null || (pgd.IsActive == true && dpg.IsActive == true))
                                 orderby item.Name
                                 select new
                                 {
                                     item.Id,
                                     item.Name,
                                     Type = itemType.Name,
                                     VolumeInMl = item.Volume,
                                     item.QuantityInPack,
                                     Image = !string.IsNullOrEmpty(item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                              ? GetImageAsBase64(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                              : null,
                                     RetailPrice = pgd != null ? pgd.NetDistributorPrice : pgd.RetailPrice
                                 }).ToList();

                    var result = query.ToList();


                    return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        //[HttpPost]
        //[Route("SaveShopOrder")]
        //public async Task<ActionResult<string>> SaveShopOrder(SaveShopOrderCommand command)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
        //        }
        //        else
        //        {
        //            if (IsValidToken(Request.Headers.Authorization) == false)
        //            {
        //                return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
        //            }
        //            if (command.AppDateTime.Date != DateTime.Now.Date)
        //            {
        //                return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
        //            }


        //            Entities.Models.Order _order = new Entities.Models.Order();
        //            _order.OrderStatusId = (long)OrderStatusEnum.OrderCreate;
        //            _order.ShopId = command.ShopId;
        //            _order.DSFId = new Guid(command.DSFId);
        //            _order.CreatedById = new Guid(command.UserId);
        //            _order.CreatedDate = DateTime.Now;
        //            unitOfWork.Repository<Entities.Models.Order>().Add(_order);
        //            unitOfWork.SaveChanges();

        //            foreach (var item in command.OrderItemCommandList)
        //            {
        //                if (item.OrderQuantity > 0)
        //                {
        //                    Entities.Models.OrderItems _orderItems = new Entities.Models.OrderItems();
        //                    _orderItems.IsActive = true;
        //                    _orderItems.IsDelete = false;
        //                    _orderItems.OrderId = _order.Id;
        //                    _orderItems.ItemId = item.ProductId;
        //                    _orderItems.Quantity = item.OrderQuantity;
        //                    _orderItems.CreatedById = new Guid(command.UserId);
        //                    _orderItems.CreatedDate = DateTime.Now;
        //                    _orderItems.TradePrice = item.TradePrice;
        //                    unitOfWork.Repository<Entities.Models.OrderItems>().Add(_orderItems);
        //                    unitOfWork.SaveChanges();
        //                }
        //            }

        //            OrderProcess process = new OrderProcess();
        //            process.OrderId = _order.Id;
        //            process.FromStatusId = null;
        //            process.ToStatusId = _order.OrderStatusId;
        //            process.Comments = "New Order Created";
        //            process.CreatedById = new Guid(command.UserId);
        //            process.CreatedDate = DateTime.Now;
        //            unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);
        //            unitOfWork.SaveChanges();

        //            return this.Result(ResponseStatus.OK, _order.Id, "Order Placed Successfully");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
        //    }
        //}

        [HttpGet]
        [Route("GetShopOrderDetailsByOrderId")]
        public async Task<ActionResult<string>> GetShopOrderDetailsByOrderId([FromQuery] long OrderId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (OrderId == 0)
                    {
                        return this.Result(ResponseStatus.ValidationFailed, null, "order Id is Compulsory");
                    }

                    var query = from ord in unitOfWork.Repository<Entities.Models.Order>().GetAll()
                                where ord.IsActive && !ord.IsDelete && ord.Id == OrderId
                                join os in unitOfWork.Repository<Status>().GetAll() on ord.OrderStatusId equals os.Id into statusGroup
                                from os in statusGroup.DefaultIfEmpty() // Left join to get order status
                                join dsf in unitOfWork.Repository<AspNetUsers>().GetAll() on ord.DSFId equals dsf.Id into dsfGroup
                                from dsf in dsfGroup.DefaultIfEmpty() // Left join to get DSF
                                join usr in unitOfWork.Repository<AspNetUsers>().GetAll() on ord.CreatedById equals usr.Id into userGroup
                                from usr in userGroup.DefaultIfEmpty() // Left join to get created by user
                                join shp in unitOfWork.Repository<Shop>().GetAll() on ord.ShopId equals shp.Id into shopGroup
                                from shp in shopGroup.DefaultIfEmpty() // Left join to get shop details
                                select new
                                {
                                    OrderId = ord.Id,
                                    OrderStatusId = ord.OrderStatusId,
                                    OrderStatus = os?.Title,
                                    ShopId = shp?.Id,
                                    ShopName = shp?.Name,
                                    ShopAddress = shp?.Address,
                                    ShopPhoneNo = shp?.PhoneNo,
                                    ShopLocation = shp?.PinLocation,
                                    OrderCreatedDate = ord.CreatedDate,
                                    OrderCreatedById = ord.CreatedById,
                                    OrderCreatedBy = usr != null ? usr.FirstName + " " + usr.LastName : null,
                                    DSFId = ord.DSFId,
                                    DSF = dsf != null ? dsf.FirstName + " " + dsf.LastName : null,
                                    Products = (from item in unitOfWork.Repository<Entities.Models.Item>().GetAll()
                                                join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll() on item.ItemTypeId equals itemType.Id
                                                join oi in unitOfWork.Repository<OrderItems>().GetAll().Where(oi => oi.OrderId == ord.Id && oi.IsActive) on item.Id equals oi.ItemId into orderItemsGroup
                                                from oi in orderItemsGroup.DefaultIfEmpty() // Left join to order items
                                                join price in unitOfWork.Repository<PriceGroupDetails>().GetAll() on item.Id equals price.ItemId into priceGroup
                                                from price in priceGroup.DefaultIfEmpty()
                                                where item.IsActive
                                                select new
                                                {
                                                    ProductId = item.Id,
                                                    ProductName = item.Name,
                                                    ProductType = itemType.Name,
                                                    VolumeInMl = item.Volume,
                                                    QuantityInPack = item.QuantityInPack,
                                                    ItemQuantity = oi?.Quantity ?? 0, // Default to 0 if no quantity
                                                    TradePrice = oi != null ? oi.TradePrice : price != null ? price.TradePrice : (decimal)0.0,
                                                    ImageName = !string.IsNullOrEmpty(item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                                                 ? GetImageAsBase64(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                                                 : null
                                                }).ToList()
                                };




                    var result = query.ToList();

                    return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("GetShopOrderStatusWiseByDateDfsId")]
        public async Task<ActionResult<string>> GetShopOrderStatusWiseByDateDfsId([FromQuery] string userId, [FromQuery] int statusId, [FromQuery] DateTime appDateTime, [FromQuery] DateTime OrderDate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        return this.Result(ResponseStatus.ValidationFailed, null, "User Id Is Compulsory");
                    }

                    var query = from ord in unitOfWork.Repository<Entities.Models.Order>().GetAll()
                                join shp in unitOfWork.Repository<Shop>().GetAll() on ord.ShopId equals shp.Id
                                join dsf in unitOfWork.Repository<AspNetUsers>().GetAll() on ord.DSFId equals dsf.Id
                                join usr in unitOfWork.Repository<AspNetUsers>().GetAll() on ord.CreatedById equals usr.Id
                                join oit in unitOfWork.Repository<OrderItems>().GetAll() on ord.Id equals oit.OrderId
                                join item in unitOfWork.Repository<Entities.Models.Item>().GetAll() on oit.ItemId equals item.Id
                                join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll()
                                    on item.ItemTypeId equals itemType.Id
                                //join att in unitOfWork.Repository<Entities.Models.Attachments>().GetAll() on prd.Id equals att.ProductId
                                join os in unitOfWork.Repository<Status>().GetAll() on ord.OrderStatusId equals os.Id
                                   into osJoin
                                from os in (statusId > 0 ? osJoin.Where(o => o.Id == statusId) : osJoin.DefaultIfEmpty())
                                where ord.IsActive == true && ord.IsDelete == false
                                      && oit.IsActive == true && oit.IsDelete == false
                                      && ord.DSFId == new Guid(userId) // Specific DSFId
                                      && ord.CreatedDate.Value.Date == OrderDate.Date // Matches today's date
                                orderby ord.Id descending
                                group new
                                {
                                    ProductName = item.Name,
                                    ProductType = itemType.Name,
                                    VolumeInMl = item.Volume,
                                    QuantityInPack = item.QuantityInPack,
                                    ItemQuantity = oit.Quantity,
                                    TradePrice = oit.TradePrice,
                                    ImageName = item.Image
                                }
                                by new
                                {
                                    ord.Id,
                                    ord.OrderStatusId,
                                    OrderStatus = os.Title,
                                    ord.ShopId,
                                    ShopName = shp.Name,
                                    ShopAddress = shp.Address,
                                    ShopPhoneNo = shp.PhoneNo,
                                    ShopLocation = shp.PinLocation,
                                    OrderCreatedDate = ord.CreatedDate,
                                    OrderCreatedById = ord.CreatedById,
                                    OrderCreatedBy = usr.FirstName + " " + usr.LastName,
                                    ord.DSFId,
                                    DSF = dsf.FirstName + " " + dsf.LastName
                                } into orderGroup
                                select new
                                {
                                    OrderId = orderGroup.Key.Id,
                                    OrderStatusId = orderGroup.Key.OrderStatusId,
                                    OrderStatus = orderGroup.Key.OrderStatus,
                                    ShopId = orderGroup.Key.ShopId,
                                    ShopName = orderGroup.Key.ShopName,
                                    ShopAddress = orderGroup.Key.ShopAddress,
                                    ShopPhoneNo = orderGroup.Key.ShopPhoneNo,
                                    ShopLocation = orderGroup.Key.ShopLocation,
                                    OrderCreatedDate = orderGroup.Key.OrderCreatedDate,
                                    OrderCreatedById = orderGroup.Key.OrderCreatedById,
                                    OrderCreatedBy = orderGroup.Key.OrderCreatedBy,
                                    DSFId = orderGroup.Key.DSFId,
                                    DSF = orderGroup.Key.DSF,
                                    Products = orderGroup.Select(p => new
                                    {
                                        p.ProductName,
                                        p.ProductType,
                                        p.VolumeInMl,
                                        p.QuantityInPack,
                                        p.ItemQuantity,
                                        p.TradePrice,
                                        ImageName = !string.IsNullOrEmpty(p.ImageName) && System.IO.File.Exists(Path.Combine(Localcontainer, p.ImageName.TrimStart('/')))
                                                   ? GetImageAsBase64(Path.Combine(Localcontainer, p.ImageName.TrimStart('/')))
                                                   : null
                                    }).ToList()
                                };



                    var result = query.ToList();


                    return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        //[HttpGet]
        //[Route("GetShopOrderStatusWiseByDateSupId")]
        //public async Task<ActionResult<string>> GetShopOrderStatusWiseByDateSupId([FromQuery] string userId, [FromQuery] int statusId, [FromQuery] DateTime appDateTime, [FromQuery] DateTime OrderDate)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
        //        }
        //        else
        //        {
        //            if (IsValidToken(Request.Headers.Authorization) == false)
        //            {
        //                return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
        //            }
        //            if (appDateTime.Date != DateTime.Now.Date)
        //            {
        //                return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
        //            }
        //            if (string.IsNullOrWhiteSpace(userId))
        //            {
        //                return this.Result(ResponseStatus.DateNotMatch, null, "User Id Is Compulsory");
        //            }
        //            //if (1 == 2)
        //            if (statusId == (long)OrderStatusEnum.OrderDispatched || statusId == (long)OrderStatusEnum.OrderReceived)
        //            {
        //                var query = from ut in unitOfWork.Repository<UserTerritory>().GetAll()
        //                            join shp in unitOfWork.Repository<Shop>().GetAll() on ut.TerritoryId equals shp.TerritoryId
        //                            join ord in unitOfWork.Repository<Entities.Models.Order>().GetAll() on shp.Id equals ord.ShopId
        //                            join dsf in unitOfWork.Repository<AspNetUsers>().GetAll() on ord.DSFId equals dsf.Id
        //                            join usr in unitOfWork.Repository<AspNetUsers>().GetAll() on ord.CreatedById equals usr.Id
        //                            join oit in unitOfWork.Repository<OrderItems>().GetAll() on ord.Id equals oit.OrderId
        //                            join item in unitOfWork.Repository<Entities.Models.Item>().GetAll() on oit.ItemId equals item.Id
        //                            join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll()
        //                                on item.ItemTypeId equals itemType.Id
        //                            join os in unitOfWork.Repository<Status>().GetAll() on ord.OrderStatusId equals os.Id
        //                              into osJoin
        //                            from os in (statusId > 0 ? osJoin.Where(o => o.Id == statusId) : osJoin.DefaultIfEmpty())
        //                            join dod in unitOfWork.Repository<DispatchOrderDetails>().GetAll()
        //                                    on ord.Id equals dod.OrderId into dodJoin
        //                            from dod in dodJoin.DefaultIfEmpty()
        //                            where ord.IsActive == true && ord.IsDelete == false
        //                                  && oit.IsActive == true && oit.IsDelete == false
        //                                  && ut.UserId == new Guid(userId) // Specific UserId
        //                                  && ord.CreatedDate?.Date == OrderDate.Date // Matches today's date
        //                            orderby ord.Id descending // Order by OrderId descending
        //                            group new
        //                            {
        //                                ProductName = item.Name,
        //                                ProductType = itemType.Name,
        //                                VolumeInMl = item.Volume,
        //                                QuantityInPack = item.QuantityInPack,
        //                                ItemQuantity = oit.Quantity,
        //                                TradePrice = oit.TradePrice,
        //                                ImageName = item.Image
        //                            }
        //                            by new
        //                            {
        //                                ord.Id,
        //                                ord.OrderStatusId,
        //                                OrderStatus = os.Title,
        //                                ord.ShopId,
        //                                ShopName = shp.Name,
        //                                ShopAddress = shp.Address,
        //                                ShopPhoneNo = shp.PhoneNo,
        //                                ShopLocation = shp.PinLocation,
        //                                OrderCreatedDate = ord.CreatedDate,
        //                                OrderCreatedById = ord.CreatedById,
        //                                OrderCreatedBy = usr.FirstName + " " + usr.LastName,
        //                                ord.DSFId,
        //                                DSF = dsf.FirstName + " " + dsf.LastName,

        //                                DeliveryDateTime = dod != null ? dod.DeliveryDateTime : null,
        //                                VehicleNo = dod != null ? dod.VehicleNo : null,
        //                                DriverName = dod != null ? dod.DriverName : null,
        //                                DriverPhoneNo = dod != null ? dod.DriverPhoneNo : null,
        //                                DeliveryChallanCode = dod != null ? dod.DeliveryChallanCode : null

        //                            } into orderGroup
        //                            select new
        //                            {
        //                                OrderId = orderGroup.Key.Id,
        //                                OrderStatusId = orderGroup.Key.OrderStatusId,
        //                                OrderStatus = orderGroup.Key.OrderStatus,
        //                                ShopId = orderGroup.Key.ShopId,
        //                                ShopName = orderGroup.Key.ShopName,
        //                                ShopAddress = orderGroup.Key.ShopAddress,
        //                                ShopPhoneNo = orderGroup.Key.ShopPhoneNo,
        //                                ShopLocation = orderGroup.Key.ShopLocation,
        //                                OrderCreatedDate = orderGroup.Key.OrderCreatedDate,
        //                                OrderCreatedById = orderGroup.Key.OrderCreatedById,
        //                                OrderCreatedBy = orderGroup.Key.OrderCreatedBy,
        //                                DSFId = orderGroup.Key.DSFId,
        //                                DSF = orderGroup.Key.DSF,
        //                                DeliveryDateTime = orderGroup.Key.DeliveryDateTime,
        //                                VehicleNo = orderGroup.Key.VehicleNo,
        //                                DriverName = orderGroup.Key.DriverName,
        //                                DriverPhoneNo = orderGroup.Key.DriverPhoneNo,
        //                                DeliveryChallanCode = orderGroup.Key.DeliveryChallanCode,
        //                                Products = orderGroup.Select(p => new
        //                                {
        //                                    p.ProductName,
        //                                    p.ProductType,
        //                                    p.VolumeInMl,
        //                                    p.QuantityInPack,
        //                                    p.ItemQuantity,
        //                                    p.TradePrice,
        //                                    ImageName = !string.IsNullOrEmpty(p.ImageName) && System.IO.File.Exists(Path.Combine(Localcontainer, p.ImageName.TrimStart('/')))
        //                                               ? GetImageAsBase64(Path.Combine(Localcontainer, p.ImageName.TrimStart('/')))
        //                                               : null
        //                                }).ToList()
        //                            };

        //                var result = query.ToList();


        //                return this.Result(ResponseStatus.OK, result, result.Count().ToString());
        //            }
        //            else
        //            {
        //                var query = from ut in unitOfWork.Repository<UserTerritory>().GetAll()
        //                            join shp in unitOfWork.Repository<Shop>().GetAll() on ut.TerritoryId equals shp.TerritoryId
        //                            join ord in unitOfWork.Repository<Entities.Models.Order>().GetAll() on shp.Id equals ord.ShopId
        //                            join dsf in unitOfWork.Repository<AspNetUsers>().GetAll() on ord.DSFId equals dsf.Id
        //                            join usr in unitOfWork.Repository<AspNetUsers>().GetAll() on ord.CreatedById equals usr.Id
        //                            join oit in unitOfWork.Repository<OrderItems>().GetAll() on ord.Id equals oit.OrderId
        //                            join item in unitOfWork.Repository<Entities.Models.Item>().GetAll() on oit.ItemId equals item.Id
        //                            join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll()
        //                                on item.ItemTypeId equals itemType.Id
        //                            join os in unitOfWork.Repository<Status>().GetAll() on ord.OrderStatusId equals os.Id
        //                              into osJoin
        //                            from os in (statusId > 0 ? osJoin.Where(o => o.Id == statusId) : osJoin.DefaultIfEmpty())
        //                            where ord.IsActive == true && ord.IsDelete == false
        //                                  && oit.IsActive == true && oit.IsDelete == false
        //                                  && ut.UserId == new Guid(userId) // Specific UserId
        //                                  && ord.CreatedDate?.Date == OrderDate.Date // Matches today's date
        //                            orderby ord.Id descending // Order by OrderId descending
        //                            group new
        //                            {
        //                                ProductName = item.Name,
        //                                ProductType = itemType.Name,
        //                                VolumeInMl = item.Volume,
        //                                QuantityInPack = item.QuantityInPack,
        //                                ItemQuantity = oit.Quantity,
        //                                TradePrice = oit.TradePrice,
        //                                ImageName = item.Image
        //                            }
        //                            by new
        //                            {
        //                                ord.Id,
        //                                ord.OrderStatusId,
        //                                OrderStatus = os.Title,
        //                                ord.ShopId,
        //                                ShopName = shp.Name,
        //                                ShopAddress = shp.Address,
        //                                ShopPhoneNo = shp.PhoneNo,
        //                                ShopLocation = shp.PinLocation,
        //                                OrderCreatedDate = ord.CreatedDate,
        //                                OrderCreatedById = ord.CreatedById,
        //                                OrderCreatedBy = usr.FirstName + " " + usr.LastName,
        //                                ord.DSFId,
        //                                DSF = dsf.FirstName + " " + dsf.LastName
        //                            } into orderGroup
        //                            select new
        //                            {
        //                                OrderId = orderGroup.Key.Id,
        //                                OrderStatusId = orderGroup.Key.OrderStatusId,
        //                                OrderStatus = orderGroup.Key.OrderStatus,
        //                                ShopId = orderGroup.Key.ShopId,
        //                                ShopName = orderGroup.Key.ShopName,
        //                                ShopAddress = orderGroup.Key.ShopAddress,
        //                                ShopPhoneNo = orderGroup.Key.ShopPhoneNo,
        //                                ShopLocation = orderGroup.Key.ShopLocation,
        //                                OrderCreatedDate = orderGroup.Key.OrderCreatedDate,
        //                                OrderCreatedById = orderGroup.Key.OrderCreatedById,
        //                                OrderCreatedBy = orderGroup.Key.OrderCreatedBy,
        //                                DSFId = orderGroup.Key.DSFId,
        //                                DSF = orderGroup.Key.DSF,
        //                                Products = orderGroup.Select(p => new
        //                                {
        //                                    p.ProductName,
        //                                    p.ProductType,
        //                                    p.VolumeInMl,
        //                                    p.QuantityInPack,
        //                                    p.ItemQuantity,
        //                                    p.TradePrice,
        //                                    ImageName = !string.IsNullOrEmpty(p.ImageName) && System.IO.File.Exists(Path.Combine(Localcontainer, p.ImageName.TrimStart('/')))
        //                                               ? GetImageAsBase64(Path.Combine(Localcontainer, p.ImageName.TrimStart('/')))
        //                                               : null
        //                                }).ToList()
        //                            };
        //                var result = query.ToList();


        //                return this.Result(ResponseStatus.OK, result, result.Count().ToString());
        //            }



        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
        //    }
        //}


        //[HttpPost]
        //[Route("UpdateShopOrderStatus")]
        //public async Task<ActionResult<string>> UpdateShopOrderStatus(UpdateOrderStatusCommand command)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
        //        }
        //        else
        //        {
        //            if (IsValidToken(Request.Headers.Authorization) == false)
        //            {
        //                return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
        //            }
        //            if (command.AppDateTime.Date != DateTime.Now.Date)
        //            {
        //                return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
        //            }

        //            var shopOrder = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsNoTrackingAsync(y => y.Id == command.OrderId);
        //            if (shopOrder == null)
        //            {
        //                return this.Result(ResponseStatus.NoContent, null, "Order Not Found");
        //            }

        //            shopOrder.OrderStatusId = command.ToStatusId;
        //            shopOrder.ModifiedDate = DateTime.Now;
        //            if (command.ToStatusId == (long)OrderStatusEnum.OrderDeleted)
        //            {
        //                //Deleted
        //                shopOrder.IsActive = false;
        //                shopOrder.IsDelete = true;
        //            }
        //            shopOrder.ModifiedById = new Guid(command.UserId);
        //            unitOfWork.Repository<Entities.Models.Order>().Update(shopOrder);

        //            OrderProcess process = new OrderProcess();
        //            process.OrderId = command.OrderId;
        //            process.FromStatusId = command.FromStatusId;
        //            process.ToStatusId = command.ToStatusId;
        //            process.Comments = command.Comments;
        //            process.CreatedById = new Guid(command.UserId);
        //            process.CreatedDate = DateTime.Now;
        //            unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);

        //            var check = await unitOfWork.SaveChangesAsync();

        //            if (check > 0)
        //            {

        //                return this.Result(ResponseStatus.OK, command.OrderId, "Order Status update Successfully");
        //            }
        //            else
        //            {

        //                return this.Result(ResponseStatus.Error, command.OrderId, "Order Status update failed");
        //            }


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
        //    }
        //}

        [HttpGet]
        [Route("GetOrderProcessByOrderId")]
        public async Task<ActionResult<string>> GetOrderProcessByOrderId([FromQuery] long orderId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (orderId == 0)
                    {
                        return this.Result(ResponseStatus.ValidationFailed, null, "Order Id Is Compulsory");
                    }


                    var query = (from op in unitOfWork.Repository<OrderProcess>().GetAll()
                                 join osf in unitOfWork.Repository<Status>().GetAll() on op.FromStatusId equals osf.Id into fromStatuses
                                 from osf in fromStatuses.DefaultIfEmpty() // Left join on FromStatus
                                 join ost in unitOfWork.Repository<Status>().GetAll() on op.ToStatusId equals ost.Id into toStatuses
                                 from ost in toStatuses.DefaultIfEmpty() // Left join on ToStatus
                                 join usr in unitOfWork.Repository<AspNetUsers>().GetAll() on op.CreatedById equals usr.Id // Join AspNetUsers
                                 where op.OrderId == orderId && op.IsActive == true
                                 orderby op.Id ascending
                                 select new
                                 {
                                     FromStatus = osf != null ? osf.Title : string.Empty,
                                     ToStatus = ost != null ? ost.Title : string.Empty,
                                     op.Comments,
                                     UpdateBy = usr.FirstName + " " + usr.LastName,
                                     op.CreatedDate
                                 }).ToList()
                                  ;


                    var result = query.ToList();


                    return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        //[HttpPost]
        //[Route("DispatchShopOrder")]
        //public async Task<ActionResult<string>> DispatchShopOrder(DispatchShopOrderCommand command)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
        //        }
        //        else
        //        {
        //            if (IsValidToken(Request.Headers.Authorization) == false)
        //            {
        //                return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
        //            }
        //            if (command.AppDateTime.Date != DateTime.Now.Date)
        //            {
        //                return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
        //            }
        //            var shopOrder = await unitOfWork.Repository<ERP.Entities.Models.Order>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && x.Id == command.OrderId, null, null, "OrderItems");

        //            if (shopOrder != null && shopOrder.OrderStatusId == (long)OrderStatusEnum.OrderConfirm)
        //            {
        //                foreach (var item in shopOrder.OrderItems)
        //                {
        //                    var shopOrderItem = await unitOfWork.Repository<OrderItems>().GetFirstAsNoTrackingAsync(x => x.Id == item.Id);
        //                    shopOrderItem.ShippedQuantity = item.Quantity;
        //                    shopOrderItem.CustomTradePrice = item.TradePrice;
        //                    shopOrderItem.ModifiedDate = DateTime.Now;
        //                    shopOrderItem.ModifiedById = new Guid(command.UserId);
        //                    unitOfWork.Repository<OrderItems>().Update(shopOrderItem);
        //                }

        //                DispatchOrderDetails _dispatchShopOrderDetails = new DispatchOrderDetails();
        //                _dispatchShopOrderDetails.Id = 0;
        //                _dispatchShopOrderDetails.OrderId = command.OrderId;
        //                _dispatchShopOrderDetails.DeliveryDateTime = command.DeliveryDateTime;
        //                _dispatchShopOrderDetails.VehicleNo = command.VehicleNo;
        //                _dispatchShopOrderDetails.DriverName = command.DriverName;
        //                _dispatchShopOrderDetails.DriverPhoneNo = command.DriverPhoneNo;
        //                _dispatchShopOrderDetails.DeliveryChallanCode = await generateDelieveryChallanCodeAsync(command.OrderId);
        //                _dispatchShopOrderDetails.CreatedDate = DateTime.Now;
        //                _dispatchShopOrderDetails.CreatedById = new Guid(command.UserId);
        //                unitOfWork.Repository<DispatchOrderDetails>().Add(_dispatchShopOrderDetails);


        //                OrderProcess process = new OrderProcess();
        //                process.OrderId = command.OrderId;
        //                process.FromStatusId = (long)OrderStatusEnum.OrderConfirm;
        //                process.ToStatusId = (long)OrderStatusEnum.OrderDispatched;
        //                process.Comments = command.Comments;
        //                process.CreatedById = new Guid(command.UserId);
        //                process.CreatedDate = DateTime.Now;
        //                unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);


        //                var orderToUpdate = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsNoTrackingAsync(x => x.Id == command.OrderId);
        //                orderToUpdate.OrderStatusId = (long)OrderStatusEnum.OrderDispatched;
        //                orderToUpdate.ModifiedDate = DateTime.Now;
        //                orderToUpdate.ModifiedById = new Guid(command.UserId);
        //                unitOfWork.Repository<Entities.Models.Order>().Update(orderToUpdate);

        //                var check = await unitOfWork.SaveChangesAsync();

        //                if (check > 0)
        //                {

        //                    return this.Result(ResponseStatus.OK, command.OrderId, "Order dispatch successfully");
        //                }
        //                else
        //                {

        //                    return this.Result(ResponseStatus.Error, command.OrderId, "Order dispatch failed");
        //                }
        //            }
        //            else
        //            {
        //                return this.Result(ResponseStatus.RecordNotFound, null, "Order Not Found");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
        //    }
        //}

        //private async Task<string> generateDelieveryChallanCodeAsync(long shopOrderId)
        //{
        //    string newNo = "";
        //    if (await unitOfWork.Repository<DispatchOrderDetails>().GetExistsAsync())
        //    {
        //        var checkExistingId = unitOfWork.Repository<DispatchOrderDetails>().GetAllAsync().Result.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
        //        newNo = "KCS-" + shopOrderId + "-" + checkExistingId;
        //    }
        //    else
        //    {
        //        newNo = "KCS-" + shopOrderId + "-1";
        //    }
        //    return newNo;
        //}

        //[HttpPost]
        //[Route("ReceiveShopOrder")]
        //public async Task<ActionResult<string>> ReceiveShopOrder(ReceiveShopOrderCommand command)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
        //        }
        //        else
        //        {
        //            if (IsValidToken(Request.Headers.Authorization) == false)
        //            {
        //                return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
        //            }
        //            if (command.AppDateTime.Date != DateTime.Now.Date)
        //            {
        //                return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
        //            }
        //            var shopOrder = await unitOfWork.Repository<ERP.Entities.Models.Order>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && x.Id == command.OrderId, null, null, "DispatchOrderDetails");

        //            if (shopOrder != null && shopOrder.OrderStatusId == (long)OrderStatusEnum.OrderDispatched && shopOrder.DispatchOrderDetails != null && shopOrder.DispatchOrderDetails.Count() > 0)
        //            {
        //                if (shopOrder.DispatchOrderDetails.FirstOrDefault().DeliveryChallanCode != command.DeliveryChallanCode)
        //                {
        //                    return this.Result(ResponseStatus.ValidationFailed, null, "Delivery challan code not matched");
        //                }

        //                OrderProcess process = new OrderProcess();
        //                process.OrderId = command.OrderId;
        //                process.FromStatusId = (long)OrderStatusEnum.OrderDispatched;
        //                process.ToStatusId = (long)OrderStatusEnum.OrderReceived;
        //                process.Comments = command.Comments;
        //                process.CreatedById = new Guid(command.UserId);
        //                process.CreatedDate = DateTime.Now;
        //                unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);

        //                var orderToUpdate = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsNoTrackingAsync(x => x.Id == command.OrderId);
        //                orderToUpdate.OrderStatusId = (long)OrderStatusEnum.OrderReceived;
        //                orderToUpdate.ModifiedDate = DateTime.Now;
        //                orderToUpdate.ModifiedById = new Guid(command.UserId);
        //                unitOfWork.Repository<Entities.Models.Order>().Update(orderToUpdate);

        //                var check = await unitOfWork.SaveChangesAsync();

        //                if (check > 0)
        //                {
        //                    return this.Result(ResponseStatus.OK, command.OrderId, "Order received successfully");
        //                }
        //                else
        //                {

        //                    return this.Result(ResponseStatus.Error, command.OrderId, "Order dispatch failed");
        //                }
        //            }
        //            else
        //            {
        //                return this.Result(ResponseStatus.RecordNotFound, null, "Order Not Found");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
        //    }
        //}
        #endregion

        #region ASD EndPoints

        [HttpGet]
        [Route("GetAllTerritoryShopByUserId")]
        public async Task<ActionResult<string>> GetAllTerritoryShopByUserId([FromQuery] string userId, [FromQuery] double lat, [FromQuery] double lng, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }

                    // Assuming you have the necessary repositories in place
                    var currentLat = lat;
                    var currentLng = lng;

                    var result = (
                              from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                              join sp in unitOfWork.Repository<Shop>().GetAll()
                                  on ut.TerritoryId equals sp.TerritoryId
                              join ord in unitOfWork.Repository<Entities.Models.Order>().GetAll()
                                  .Where(o => o.IsActive && o.CreatedDate.HasValue && o.CreatedDate.Value.Date == DateTime.Now.Date)
                                  on sp.Id equals ord.ShopId into ordGroup
                              from ord in ordGroup.DefaultIfEmpty()
                              join tord in unitOfWork.Repository<Entities.Models.Order>().GetAll()
                                   .Where(to => to.IsActive && to.CreatedDate.HasValue)
                                   on sp.Id equals tord.ShopId into tordGroup
                              from tord in tordGroup.DefaultIfEmpty()

                              where ut.IsActive && sp.IsActive && sp.IsVerified == true
                                    && ut.UserId == new Guid(userId)

                              let shopLat = JsonDocument.Parse(sp.PinLocation).RootElement.GetProperty("lat").GetDouble()
                              let shopLng = JsonDocument.Parse(sp.PinLocation).RootElement.GetProperty("lng").GetDouble()
                              let distanceInMeters = 6371000 * Math.Acos(
                                  Math.Cos(DegreeToRadian(currentLat)) * Math.Cos(DegreeToRadian(shopLat)) *
                                  Math.Cos(DegreeToRadian(shopLng) - DegreeToRadian(currentLng)) +
                                  Math.Sin(DegreeToRadian(currentLat)) * Math.Sin(DegreeToRadian(shopLat))
                              )

                              // Group by shop information
                              group new { ord, tord } by new
                              {
                                  sp.Id,
                                  sp.Name,
                                  sp.OwnerName,
                                  sp.Address,
                                  sp.PinLocation,
                                  sp.OpeningTime,
                                  sp.ClosingTime,
                                  sp.PhoneNo,
                                  shopLat,
                                  shopLng,
                                  distanceInMeters
                              } into g

                              select new
                              {
                                  ShopId = g.Key.Id,
                                  ShopName = g.Key.Name,
                                  g.Key.OwnerName,
                                  ShopAddress = g.Key.Address,
                                  g.Key.PinLocation,
                                  g.Key.OpeningTime,
                                  g.Key.ClosingTime,
                                  g.Key.PhoneNo,
                                  ShopLat = g.Key.shopLat,
                                  ShopLng = g.Key.shopLng,
                                  DistanceInMeters = g.Key.distanceInMeters,

                                  // Conditional distance formatting
                                  FormattedDistance = g.Key.distanceInMeters < 1000
                                              ? g.Key.distanceInMeters.ToString("F0")
                                              : (g.Key.distanceInMeters / 1000.0).ToString("F2"),
                                  FormattedDistanceUnit = g.Key.distanceInMeters < 1000
                                              ? "m"
                                              : "km",

                                  // Check if there's an order for the shop today
                                  IsOrder = g.Count(x => x.ord != null) > 0 ? "Yes" : "No",

                                  // Count of total orders (tord)
                                  //TotalOrder = g.Count(x => x.tord != null)
                                  TotalOrder = g.Select(x => x.tord?.Id).Where(id => id != null).Distinct().Count()
                              }
                          )
                          .OrderByDescending(x => x.IsOrder == "Yes" ? 1 : 0)
                          .ThenBy(x => x.DistanceInMeters)
                          .ToList();

                    return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("GetTerritoryTargetByTargetId")]
        public async Task<ActionResult<string>> GetTerritoryTargetByTargetId([FromQuery] string userId, [FromQuery] DateTime targetMonth, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));

                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }

                    var activeTerritoryId = unitOfWork.Repository<UserTerritory>()
                                            .GetAll()
                                            .Where(subUt => subUt.UserId == new Guid(userId) && subUt.IsActive)
                                            .Select(subUt => subUt.TerritoryId)
                                            .Distinct()
                                            .FirstOrDefault();


                    var RSM = await unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name == "ZSM");
                    var SalesSupervisor = await unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name == "ASE");
                    var Admin = await unitOfWork.Repository<AspNetRoles>().GetFirstAsNoTrackingAsync(x => x.Name == "Admin");

                    var territoryDsfList = await unitOfWork.Repository<Entities.Models.UserTerritory>()
                        .GetAsync(x => x.TerritoryId == activeTerritoryId && x.IsActive == true && x.User.AspNetUserRoles.FirstOrDefault().RoleId != RSM.Id && x.User.AspNetUserRoles.FirstOrDefault().RoleId != SalesSupervisor.Id && x.User.AspNetUserRoles.FirstOrDefault().RoleId != Admin.Id, null, null, "User.AspNetUserRoles");

                    var territoryTargetsByZoneId = await unitOfWork.Repository<Entities.Models.SalesTarget>()
                        .GetAsync(x => x.IsActive == true && x.TargetMonth.Month == targetMonth.Month, null, null, "Territory");

                    #region territoryTargets

                    var territoryTargetsofThisMonth = await unitOfWork.Repository<Entities.Models.SalesTarget>()
                        .GetAsync(x => x.IsActive == true && x.IsDelete == false && x.UserId == null && x.TargetMonth.Month == targetMonth.Month, null, null, "Territory");

                    var achievedTargetsofThisMonth = await unitOfWork.Repository<Entities.Models.Order>()
                       .GetAsync(
                       x => x.IsActive == true &&
                       x.IsDelete == false &&
                       x.DSFId == null &&
                       x.Dealership.TerritoryId == activeTerritoryId &&
                       x.CreatedDate.Value.Month == targetMonth.Month &&
                       x.OrderStatusId == (long)OrderStatusEnum.OrderReceived,
                       null,
                       null,
                       "Dealership,OrderItems"); // Make sure to include "OrderItems" in the include if it's not already

                    // Sum the ShippedQuantity from all OrderItems of the achievedTargetsofThisMonth
                    int shippedQuantity = achievedTargetsofThisMonth
                        .SelectMany(x => x.OrderItems)   // Flatten the OrderItems collection across all orders
                        .Sum(y => y.ShippedQuantity).Value;    // Sum the ShippedQuantity from each OrderItem




                    var territoryTargets = mapper.Map<GetSalesTarget>(territoryTargetsofThisMonth.FirstOrDefault());
                    if (territoryTargets == null)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Territory Targets not found of this month");
                    }
                    territoryTargets.AchievedTarget = shippedQuantity;
                    #endregion

                    #region DFS targets

                    var currentTargetByUserId = territoryTargetsByZoneId
                        .GroupBy(x => x.UserId)
                        .Select(g => g.FirstOrDefault())
                        .ToDictionary(x => x.UserId, x => x);

                    List<GetSalesTarget> DSFTargetList = new();
                    foreach (var item in territoryDsfList)
                    {
                        GetSalesTarget DSFTarget = new();
                        DSFTarget = mapper.Map<GetSalesTarget>(currentTargetByUserId.TryGetValue(item.UserId, out var target) ? target : null);


                        var achievedTargetsofThisMonth1 = await unitOfWork.Repository<Entities.Models.Order>()
                         .GetAsync(
                         x => x.IsActive == true &&
                            x.IsDelete == false &&
                            x.DSFId == item.UserId &&
                            x.Shop.TerritoryId == activeTerritoryId &&
                            x.CreatedDate.Value.Month == targetMonth.Month &&
                            x.OrderStatusId == (long)OrderStatusEnum.OrderReceived,
                         null,
                         null,
                         "Shop,OrderItems"); // Make sure to include "OrderItems" in the include if it's not already
                        // Sum the ShippedQuantity from all OrderItems of the achievedTargetsofThisMonth
                        int shippedQuantityDfs = achievedTargetsofThisMonth1
                            .SelectMany(x => x.OrderItems)   // Flatten the OrderItems collection across all orders
                            .Sum(y => y.ShippedQuantity).Value;    // Sum the ShippedQuantity from each OrderItem


                        if (DSFTarget != null)
                        {
                            DSFTarget.AchievedTarget = shippedQuantityDfs;
                            //DSFTarget.Territory.Dealership = null;
                            DSFTargetList.Add(DSFTarget);
                        }
                    }

                    #endregion

                    GetTerritoryTarget getTerritoryTarget = new();
                    getTerritoryTarget.TerritoryTarget = territoryTargets;
                    getTerritoryTarget.Target = DSFTargetList;


                    return this.Result(ResponseStatus.OK, getTerritoryTarget, getTerritoryTarget.Target.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }


        [HttpGet]
        [Route("GetShopOrderHistoryByShopId")]
        public async Task<ActionResult<string>> GetShopOrderHistoryByShopId([FromQuery] long shopId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    var result = (
                        from ord in unitOfWork.Repository<Entities.Models.Order>().GetAll()
                        join ords in unitOfWork.Repository<Status>().GetAll()
                            on ord.OrderStatusId equals ords.Id
                        join usr in unitOfWork.Repository<AspNetUsers>().GetAll()
                            on ord.DSFId equals usr.Id
                        join sp in unitOfWork.Repository<Shop>().GetAll()
                            on ord.ShopId equals sp.Id
                        where ord.IsActive && ord.ShopId == shopId
                        orderby ord.ModifiedDate, ord.CreatedDate descending
                        select new
                        {
                            OrderId = ord.Id,
                            OrderStatusId = ord.OrderStatusId,
                            OrderStatus = ords.Title,
                            OrderCreatedDate = ord.CreatedDate,
                            OrderCreatedBy = usr.FirstName + " " + usr.LastName,
                            ShopName = sp.Name,
                            ShopAddress = sp.Address,
                            ShopPhoneNo = sp.PhoneNo,
                            ShopLocation = sp.PinLocation
                        }
                    ).ToList();


                    return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("UpdateShopOrderByOrderId")]
        public async Task<ActionResult<string>> UpdateShopOrderByOrderId(SaveShopOrderCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (command.OrderId == null || command.OrderId == 0)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Invalid Order Id");
                    }
                    var lObjOrderItems = from u in unitOfWork.Repository<Entities.Models.OrderItems>().GetAll()
                                         where u.OrderId == command.OrderId
                                         select u;
                    if (lObjOrderItems.Count() == 0)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Order not found");
                    }
                    foreach (var orderItems in lObjOrderItems)
                    {
                        orderItems.IsActive = false;
                        orderItems.IsDelete = true;
                        orderItems.ModifiedDate = DateTime.Now;
                        orderItems.ModifiedById = new Guid(command.UserId);

                        unitOfWork.Repository<OrderItems>().Update(orderItems);
                    }
                    unitOfWork.SaveChanges();

                    foreach (var item in command.OrderItemCommandList)
                    {
                        if (item.OrderQuantity > 0)
                        {
                            Entities.Models.OrderItems _orderItems = new Entities.Models.OrderItems();
                            _orderItems.IsActive = true;
                            _orderItems.IsDelete = false;
                            _orderItems.OrderId = (long)command.OrderId;
                            _orderItems.ItemId = item.ProductId;
                            _orderItems.Quantity = item.OrderQuantity;
                            _orderItems.CreatedById = new Guid(command.UserId);
                            _orderItems.CreatedDate = DateTime.Now;
                            _orderItems.TradePrice = item.TradePrice;
                            unitOfWork.Repository<Entities.Models.OrderItems>().Add(_orderItems);
                            unitOfWork.SaveChanges();
                        }
                    }



                    return this.Result(ResponseStatus.OK, command.OrderId, "Order update Successfully");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }
        #endregion

        #region ClearLog
        [HttpGet]
        [Route("ClearLog1")]
        public async Task<ActionResult<string>> ClearLog1([FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }

                    var connectionString = _configuration.GetConnectionString("DefaultConnectionString");
                    var builder = new SqlConnectionStringBuilder(connectionString);
                    var db = builder.InitialCatalog;
                    if (db != "Khilafat_Cola_dvp3")
                    {
                        return this.Result(ResponseStatus.Unauthorized, "Be careful,you're entering the wrong URL", "Be careful, you're entering the wrong URL");
                    }
                    //var databaseName = GetDatabaseNameFromConnectionString(connectionString);

                    //UserAttendance
                    var allUserAttendance = await unitOfWork.Repository<Entities.Models.UserAttendance>().GetAllAsync();

                    if (allUserAttendance.Count() > 0)
                    {
                        foreach (var item in allUserAttendance)
                        {
                            unitOfWork.Repository<Entities.Models.UserAttendance>().Delete(item);
                        }
                        unitOfWork.SaveChanges();
                    }


                    //Mark Shop Visit
                    var lObjMarkShopVisitAttachments = from u in unitOfWork.Repository<Attachments>().GetAll()
                                                       where u.MarkShopVisitId != null
                                                       select u;

                    if (lObjMarkShopVisitAttachments.Count() > 0)
                    {
                        foreach (var item in lObjMarkShopVisitAttachments)
                        {
                            unitOfWork.Repository<Entities.Models.Attachments>().Delete(item);
                        }
                        unitOfWork.SaveChanges();
                    }

                    var allMarkShopVisits = await unitOfWork.Repository<Entities.Models.MarkShopVisit>().GetAllAsync();

                    if (allMarkShopVisits.Count() > 0)
                    {
                        foreach (var item in allMarkShopVisits)
                        {
                            unitOfWork.Repository<Entities.Models.MarkShopVisit>().Delete(item);
                        }
                        unitOfWork.SaveChanges();
                    }

                    ////Order
                    //var lObjOrderAttachments = from u in unitOfWork.Repository<Attachments>().GetAll()
                    //                           where u.OrderId != null
                    //                           select u;

                    //if (lObjOrderAttachments.Count() > 0)
                    //{
                    //    foreach (var item in lObjOrderAttachments)
                    //    {
                    //        unitOfWork.Repository<Entities.Models.Attachments>().Delete(item);
                    //    }
                    //    unitOfWork.SaveChanges();
                    //}

                    //var allOrderItems = await unitOfWork.Repository<Entities.Models.OrderItems>().GetAllAsync();

                    //if (allOrderItems.Count() > 0)
                    //{
                    //    foreach (var item in allOrderItems)
                    //    {
                    //        unitOfWork.Repository<Entities.Models.OrderItems>().Delete(item);
                    //    }
                    //    unitOfWork.SaveChanges();
                    //}

                    //var allOrder = await unitOfWork.Repository<Entities.Models.Order>().GetAllAsync();

                    //if (allOrder.Count() > 0)
                    //{
                    //    foreach (var item in allOrder)
                    //    {
                    //        unitOfWork.Repository<Entities.Models.Order>().Delete(item);
                    //    }
                    //    unitOfWork.SaveChanges();
                    //}



                    return this.Result(ResponseStatus.OK, "Clear All APP Data", "Clear All APP Data");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }
        #endregion

        #region MyRegion
        [HttpGet]
        [Route("GetDistributorByUserId")]
        public async Task<ActionResult<string>> GetDistributorByUserId([FromQuery] string userId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Invalid User Id");
                    }

                    Expression<Func<Entities.Models.AspNetUsers, bool>> predicate = x => x.IsActive == true && x.Id == new Guid(userId);

                    List<string> thenInclude = new List<string>();
                    thenInclude.Add("AspNetUserRoles.Role");

                    Expression<Func<Entities.Models.AspNetUsers, object>>[] includes = {
                     x => x.AspNetUserRoles
                    };


                    var lObjUserEntity = unitOfWork.Repository<Entities.Models.AspNetUsers>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, thenInclude, includes);

                    if (lObjUserEntity.Item2 == 0)
                    {
                        return this.Result(ResponseStatus.RecordNotFound, null, "No Record Found");
                    }
                    string RoleLevel = GetRoleLevel(lObjUserEntity.Item1.ToList().FirstOrDefault().AspNetUserRoles.FirstOrDefault().Role.Name);

                    if (RoleLevel == "Region")
                    {
                        var result = (
                                        from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                                        join rg in unitOfWork.Repository<ERP.Entities.Models.Region>().GetAll() on ut.RegionId equals rg.Id
                                        join zn in unitOfWork.Repository<Zone>().GetAll() on rg.Id equals zn.RegionId
                                        join ar in unitOfWork.Repository<Area>().GetAll() on zn.Id equals ar.ZoneId
                                        join tr in unitOfWork.Repository<Territory>().GetAll() on ar.Id equals tr.AreaId
                                        join dis in unitOfWork.Repository<Dealership>().GetAll() on tr.Id equals dis.TerritoryId
                                        where dis.DealershipTypeId == 1 && ut.UserId == Guid.Parse(userId) &&
                                              ut.IsActive && !ut.IsDelete &&
                                              rg.IsActive && !rg.IsDelete &&
                                              zn.IsActive && !zn.IsDelete &&
                                              ar.IsActive && !ar.IsDelete &&
                                              tr.IsActive && !tr.IsDelete &&
                                              dis.IsActive && !dis.IsDelete
                                        select new
                                        {
                                            DealershipId = dis.Id,
                                            DealershipName = dis.Name,
                                            PhoneNo = dis.PhoneNo,
                                            Address = dis.Address,
                                            PinLocation = dis.PinLocation
                                        }
                                    ).ToList();
                        return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                    }
                    else if (RoleLevel == "Zone")
                    {
                        var result = (
                                from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                                join zn in unitOfWork.Repository<Zone>().GetAll() on ut.ZoneId equals zn.Id
                                join ar in unitOfWork.Repository<Area>().GetAll() on zn.Id equals ar.ZoneId
                                join tr in unitOfWork.Repository<Territory>().GetAll() on ar.Id equals tr.AreaId
                                join dis in unitOfWork.Repository<Dealership>().GetAll() on tr.Id equals dis.TerritoryId
                                where dis.DealershipTypeId == 1 && ut.UserId == Guid.Parse(userId) &&
                                      ut.IsActive && !ut.IsDelete &&
                                      zn.IsActive && !zn.IsDelete &&
                                      ar.IsActive && !ar.IsDelete &&
                                      tr.IsActive && !tr.IsDelete &&
                                      dis.IsActive && !dis.IsDelete
                                select new
                                {
                                    DealershipId = dis.Id,
                                    DealershipName = dis.Name,
                                    PhoneNo = dis.PhoneNo,
                                    Address = dis.Address,
                                    PinLocation = dis.PinLocation
                                }
                            ).ToList();
                        return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                    }
                    else if (RoleLevel == "Area")
                    {
                        var result = (
                            from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                            join ar in unitOfWork.Repository<Area>().GetAll() on ut.AreaId equals ar.Id
                            join tr in unitOfWork.Repository<Territory>().GetAll() on ar.Id equals tr.AreaId
                            join dis in unitOfWork.Repository<Dealership>().GetAll() on tr.Id equals dis.TerritoryId
                            where dis.DealershipTypeId == 1 && ut.UserId == Guid.Parse(userId) &&
                                  ut.IsActive && !ut.IsDelete &&
                                  ar.IsActive && !ar.IsDelete &&
                                  tr.IsActive && !tr.IsDelete &&
                                  dis.IsActive && !dis.IsDelete
                            select new
                            {
                                DealershipId = dis.Id,
                                DealershipName = dis.Name,
                                PhoneNo = dis.PhoneNo,
                                Address = dis.Address,
                                PinLocation = dis.PinLocation
                            }
                        ).ToList();
                        return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                    }
                    else if (RoleLevel == "Territory")
                    {
                        var result = (
                            from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                            join tr in unitOfWork.Repository<Territory>().GetAll() on ut.TerritoryId equals tr.Id
                            join dis in unitOfWork.Repository<Dealership>().GetAll() on tr.Id equals dis.TerritoryId
                            where dis.DealershipTypeId == 1 && ut.UserId == Guid.Parse(userId) &&
                                  ut.IsActive && !ut.IsDelete &&
                                  tr.IsActive && !tr.IsDelete &&
                                  dis.IsActive && !dis.IsDelete
                            select new
                            {
                                DealershipId = dis.Id,
                                DealershipName = dis.Name,
                                PhoneNo = dis.PhoneNo,
                                Address = dis.Address,
                                PinLocation = dis.PinLocation
                            }
                        ).ToList();
                        return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                    }
                    return this.Result(ResponseStatus.OK, "", "Order update Successfully");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("GetTerritoryByUserId")]
        public async Task<ActionResult<string>> GetTerritoryByUserId([FromQuery] string userId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Invalid User Id");
                    }

                    Expression<Func<Entities.Models.AspNetUsers, bool>> predicate = x => x.IsActive == true && x.Id == new Guid(userId);

                    List<string> thenInclude = new List<string>();
                    thenInclude.Add("AspNetUserRoles.Role");

                    Expression<Func<Entities.Models.AspNetUsers, object>>[] includes = {
                     x => x.AspNetUserRoles
                    };


                    var lObjUserEntity = unitOfWork.Repository<Entities.Models.AspNetUsers>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, thenInclude, includes);

                    if (lObjUserEntity.Item2 == 0)
                    {
                        return this.Result(ResponseStatus.RecordNotFound, null, "No Record Found");
                    }
                    string RoleLevel = GetRoleLevel(lObjUserEntity.Item1.ToList().FirstOrDefault().AspNetUserRoles.FirstOrDefault().Role.Name);

                    //if (RoleLevel == "Region")
                    //{
                    //    var result = (
                    //                    from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                    //                    join rg in unitOfWork.Repository<ERP.Entities.Models.Region>().GetAll() on ut.RegionId equals rg.Id
                    //                    join zn in unitOfWork.Repository<Zone>().GetAll() on rg.Id equals zn.RegionId
                    //                    join ar in unitOfWork.Repository<Area>().GetAll() on zn.Id equals ar.ZoneId
                    //                    join tr in unitOfWork.Repository<Territory>().GetAll() on ar.Id equals tr.AreaId
                    //                    join dis in unitOfWork.Repository<Dealership>().GetAll() on tr.Id equals dis.TerritoryId
                    //                    where dis.DealershipTypeId == 1 && ut.UserId == Guid.Parse(userId) &&
                    //                          ut.IsActive && !ut.IsDelete &&
                    //                          rg.IsActive && !rg.IsDelete &&
                    //                          zn.IsActive && !zn.IsDelete &&
                    //                          ar.IsActive && !ar.IsDelete &&
                    //                          tr.IsActive && !tr.IsDelete &&
                    //                          dis.IsActive && !dis.IsDelete
                    //                    select new
                    //                    {
                    //                        DealershipId = dis.Id,
                    //                        DealershipName = dis.Name,
                    //                        PhoneNo = dis.PhoneNo,
                    //                        Address = dis.Address,
                    //                        PinLocation = dis.PinLocation
                    //                    }
                    //                ).ToList();
                    //    return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                    //}
                    //else if (RoleLevel == "Zone")
                    //{
                    //    var result = (
                    //            from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                    //            join zn in unitOfWork.Repository<Zone>().GetAll() on ut.ZoneId equals zn.Id
                    //            join ar in unitOfWork.Repository<Area>().GetAll() on zn.Id equals ar.ZoneId
                    //            join tr in unitOfWork.Repository<Territory>().GetAll() on ar.Id equals tr.AreaId
                    //            join dis in unitOfWork.Repository<Dealership>().GetAll() on tr.Id equals dis.TerritoryId
                    //            where dis.DealershipTypeId == 1 && ut.UserId == Guid.Parse(userId) &&
                    //                  ut.IsActive && !ut.IsDelete &&
                    //                  zn.IsActive && !zn.IsDelete &&
                    //                  ar.IsActive && !ar.IsDelete &&
                    //                  tr.IsActive && !tr.IsDelete &&
                    //                  dis.IsActive && !dis.IsDelete
                    //            select new
                    //            {
                    //                DealershipId = dis.Id,
                    //                DealershipName = dis.Name,
                    //                PhoneNo = dis.PhoneNo,
                    //                Address = dis.Address,
                    //                PinLocation = dis.PinLocation
                    //            }
                    //        ).ToList();
                    //    return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                    //}
                    //else 
                    if (RoleLevel == "Area")
                    {
                        var result = (
                            from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                            join ar in unitOfWork.Repository<Area>().GetAll() on ut.AreaId equals ar.Id
                            join tr in unitOfWork.Repository<Territory>().GetAll() on ar.Id equals tr.AreaId
                            where ut.UserId == Guid.Parse(userId) &&
                                  ut.IsActive && !ut.IsDelete &&
                                  ar.IsActive && !ar.IsDelete &&
                                  tr.IsActive && !tr.IsDelete
                            select new
                            {
                                TerritoryId = tr.Id,
                                TerritoryName = tr.Name
                            }
                        ).ToList();
                        return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                    }
                    //else if (RoleLevel == "Territory")
                    //{
                    //    var result = (
                    //        from ut in unitOfWork.Repository<UserTerritory>().GetAll()
                    //        join tr in unitOfWork.Repository<Territory>().GetAll() on ut.TerritoryId equals tr.Id
                    //        join dis in unitOfWork.Repository<Dealership>().GetAll() on tr.Id equals dis.TerritoryId
                    //        where dis.DealershipTypeId == 1 && ut.UserId == Guid.Parse(userId) &&
                    //              ut.IsActive && !ut.IsDelete &&
                    //              tr.IsActive && !tr.IsDelete &&
                    //              dis.IsActive && !dis.IsDelete
                    //        select new
                    //        {
                    //            DealershipId = dis.Id,
                    //            DealershipName = dis.Name,
                    //            PhoneNo = dis.PhoneNo,
                    //            Address = dis.Address,
                    //            PinLocation = dis.PinLocation
                    //        }
                    //    ).ToList();
                    //    return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                    //}
                    return this.Result(ResponseStatus.OK, "", "Order update Successfully");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        private string GetRoleLevel(string RoleName)
        {
            RoleName = RoleName.ToUpper();

            if (RoleName == "RSM")
            {
                //REgion Level
                return "Region";
            }
            else if (RoleName == "ZSM")
            {
                //Zone LEvel
                return "Zone";
            }
            else if (RoleName == "ASD" || RoleName == "ASE" || RoleName == "ASM")
            {
                //Area Level
                return "Area";
            }
            else if (RoleName == "DSF" || RoleName == "Salesman")
            {
                //Territory Distributor LEvel
                return "Territory";

            }
            return string.Empty;
        }

        //[HttpPost]
        //[Route("ReceiveOrder")]
        //public async Task<ActionResult<string>> ReceiveOrder(ReceiveShopOrderCommand command)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
        //        }
        //        else
        //        {
        //            if (IsValidToken(Request.Headers.Authorization) == false)
        //            {
        //                return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
        //            }
        //            if (command.AppDateTime.Date != DateTime.Now.Date)
        //            {
        //                return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
        //            }
        //            var shopOrder = await unitOfWork.Repository<ERP.Entities.Models.Order>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && x.Id == command.OrderId, null, null, "DispatchOrderDetails");

        //            if (shopOrder != null && shopOrder.OrderStatusId == (long)OrderStatusEnum.OrderDispatched && shopOrder.DispatchOrderDetails != null && shopOrder.DispatchOrderDetails.Count() > 0)
        //            {
        //                if (shopOrder.DispatchOrderDetails.FirstOrDefault().DeliveryChallanCode != command.DeliveryChallanCode)
        //                {
        //                    return this.Result(ResponseStatus.ValidationFailed, null, "Delivery challan code not matched");
        //                }

        //                OrderProcess process = new OrderProcess();
        //                process.OrderId = command.OrderId;
        //                process.FromStatusId = (long)OrderStatusEnum.OrderDispatched;
        //                process.ToStatusId = (long)OrderStatusEnum.OrderReceived;
        //                process.Comments = command.Comments;
        //                process.CreatedById = new Guid(command.UserId);
        //                process.CreatedDate = DateTime.Now;
        //                unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);

        //                var orderToUpdate = await unitOfWork.Repository<Entities.Models.Order>().GetFirstAsNoTrackingAsync(x => x.Id == command.OrderId);
        //                orderToUpdate.OrderStatusId = (long)OrderStatusEnum.OrderReceived;
        //                orderToUpdate.ModifiedDate = DateTime.Now;
        //                orderToUpdate.ModifiedById = new Guid(command.UserId);
        //                unitOfWork.Repository<Entities.Models.Order>().Update(orderToUpdate);

        //                var check = await unitOfWork.SaveChangesAsync();

        //                if (check > 0)
        //                {
        //                    return this.Result(ResponseStatus.OK, command.OrderId, "Order received successfully");
        //                }
        //                else
        //                {

        //                    return this.Result(ResponseStatus.Error, command.OrderId, "Order dispatch failed");
        //                }
        //            }
        //            else
        //            {
        //                return this.Result(ResponseStatus.RecordNotFound, null, "Order Not Found");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
        //    }
        //}

        [HttpGet]
        [Route("GetDistOrderDetailsByOrderId")]
        public async Task<ActionResult<string>> GetDistOrderDetailsByOrderId([FromQuery] long OrderId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (appDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (OrderId == 0)
                    {
                        return this.Result(ResponseStatus.ValidationFailed, null, "order Id is Compulsory");
                    }

                    var Selectedorder = await unitOfWork.Repository<Entities.Models.Order>().GetAsync(x => x.Id == OrderId && x.IsActive == true && x.IsDelete == false);

                    if (Selectedorder == null)
                    {
                        return this.Result(ResponseStatus.ValidationFailed, null, "order Not Found");
                    }

                    var distributorPriceGroups = await unitOfWork.Repository<Entities.Models.DistributorPriceGroup>().GetAsync(x => x.DealershipId == Selectedorder.FirstOrDefault().DealershipId && x.IsActive == true && x.IsDelete == false);

                    if (distributorPriceGroups == null || distributorPriceGroups.Count() == 0)
                    {
                        return this.Result(ResponseStatus.ValidationFailed, null, "No active Distributor Price Groups found for the Selected Distributor");
                    }

                    var priceGroupIds = distributorPriceGroups.Select(x => x.PriceGroupId).ToList();


                    var query = from ord in unitOfWork.Repository<Entities.Models.Order>().GetAll()
                                where ord.IsActive && !ord.IsDelete && ord.Id == OrderId
                                join os in unitOfWork.Repository<Status>().GetAll() on ord.OrderStatusId equals os.Id into statusGroup
                                from os in statusGroup.DefaultIfEmpty() // Left join to get order status
                                join usr in unitOfWork.Repository<AspNetUsers>().GetAll() on ord.CreatedById equals usr.Id into userGroup
                                from usr in userGroup.DefaultIfEmpty() // Left join to get created by user
                                join dist in unitOfWork.Repository<Dealership>().GetAll() on ord.DealershipId equals dist.Id into distGroup
                                from dist in distGroup.DefaultIfEmpty() // Left join to get shop details
                                where dist.DealershipTypeId == 1
                                select new
                                {
                                    OrderId = ord.Id,
                                    OrderStatusId = ord.OrderStatusId,
                                    OrderStatus = os?.Title,
                                    DistributorId = dist?.Id,
                                    DistributorName = dist?.Name,
                                    DistributorAddress = dist?.Address,
                                    DistributorPhoneNo = dist?.PhoneNo,
                                    DistributorLocation = dist?.PinLocation,
                                    OrderCreatedDate = ord.CreatedDate,
                                    OrderCreatedById = ord.CreatedById,
                                    OrderCreatedBy = usr != null ? usr.FirstName + " " + usr.LastName : null,
                                    Products = (

                                               from oi in unitOfWork.Repository<OrderItems>().GetAll()
                                                .Where(oi => oi.OrderId == ord.Id && oi.IsActive)
                                               join item in unitOfWork.Repository<ERP.Entities.Models.Item>().GetAll()
                                                   on oi.ItemId equals item.Id
                                               join itemType in unitOfWork.Repository<ItemType>().GetAll()
                                                   on item.ItemTypeId equals itemType.Id
                                               join pgd in unitOfWork.Repository<PriceGroupDetails>().GetAll()
                                                   .Where(pgd => pgd.IsActive && !pgd.IsDelete && priceGroupIds.Contains(pgd.PriceGroupId.Value))
                                                   on item.Id equals pgd.ItemId into priceGroup
                                               from pgd in priceGroup.DefaultIfEmpty() // Left join to price group
                                               where item.IsActive
                                               select new
                                               {
                                                   ProductId = item.Id,
                                                   ProductName = item.Name,
                                                   ProductType = itemType.Name,
                                                   VolumeInMl = item.Volume,
                                                   QuantityInPack = item.QuantityInPack,
                                                   ItemQuantity = oi?.Quantity ?? 0, // Default to 0 if no quantity
                                                   DistributorPrice = pgd != null ? pgd.NetDistributorPrice : pgd.DistributorPrice, // Use NetDistributorPrice if available
                                                   ImageName = !string.IsNullOrEmpty(item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                                                ? GetImageAsBase64(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                                                : null
                                               }).ToList()
                                };

                    var result = query.ToList();

                    return this.Result(ResponseStatus.OK, result, result.Count().ToString());
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("UpdateDistOrderByOrderId")]
        public async Task<ActionResult<string>> UpdateDistOrderByOrderId(SaveDistOrderCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }
                    if (command.OrderId == null || command.OrderId == 0)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Invalid Order Id");
                    }
                    var lObjOrderItems = from u in unitOfWork.Repository<Entities.Models.OrderItems>().GetAll()
                                         where u.OrderId == command.OrderId
                                         select u;
                    if (lObjOrderItems.Count() == 0)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Order not found");
                    }

                    foreach (var orderItems in lObjOrderItems)
                    {
                        orderItems.IsActive = false;
                        orderItems.IsDelete = true;
                        orderItems.ModifiedDate = DateTime.Now;
                        orderItems.ModifiedById = new Guid(command.UserId);

                        unitOfWork.Repository<OrderItems>().Update(orderItems);
                    }
                    unitOfWork.SaveChanges();


                    var LobjOrder = await unitOfWork.Repository<Entities.Models.Order>().GetAsync(x => x.Id == command.OrderId && x.IsActive == true && x.IsDelete == false);
                    var distributorPriceGroups = await unitOfWork.Repository<Entities.Models.DistributorPriceGroup>().GetAsync(x => x.DealershipId == LobjOrder.ToList().FirstOrDefault().DealershipId && x.IsActive == true && x.IsDelete == false);

                    foreach (var item in command.OrderItemCommandList)
                    {
                        if (item.OrderQuantity > 0)
                        {
                            var itemPriceGroupDetails = await unitOfWork.Repository<Entities.Models.PriceGroupDetails>().GetAsync(x => x.PriceGroupId == distributorPriceGroups.FirstOrDefault().PriceGroupId && x.ItemId == item.ProductId && x.IsActive == true && x.IsDelete == false);

                            var lstitemPriceGroupDetails = itemPriceGroupDetails.ToList().FirstOrDefault();

                            Entities.Models.OrderItems _orderItems = new Entities.Models.OrderItems();
                            _orderItems.IsActive = true;
                            _orderItems.IsDelete = false;
                            _orderItems.OrderId = (long)command.OrderId;
                            _orderItems.ItemId = item.ProductId;
                            _orderItems.Quantity = item.OrderQuantity;
                            _orderItems.CreatedById = new Guid(command.UserId);
                            _orderItems.CreatedDate = DateTime.Now;
                            _orderItems.DistributorPrice = item.DistributorPrice;
                            _orderItems.DistributorPromo = lstitemPriceGroupDetails.DistributorPromo;
                            _orderItems.TradePrice = lstitemPriceGroupDetails.TradePrice;
                            _orderItems.RetailPrice = lstitemPriceGroupDetails.RetailPrice;

                            unitOfWork.Repository<Entities.Models.OrderItems>().Add(_orderItems);
                            unitOfWork.SaveChanges();
                        }
                    }
                    return this.Result(ResponseStatus.OK, command.OrderId, "Order update Successfully");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }
        [HttpGet]
        [Route("GetDistStockByDistId")]
        public async Task<ActionResult<string>> GetDistStockByDistId([FromQuery] long DistributorId, [FromQuery] DateTime appDateTime)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                if (appDateTime.Date != DateTime.Now.Date)
                {
                    return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                }

                if (DistributorId == 0)
                {
                    return this.Result(ResponseStatus.ValidationFailed, null, "Distributor Id is Compulsory");
                }

                var distributor = await unitOfWork.Repository<Dealership>().GetFirstAsync(d => d.DealershipTypeId == 1 && d.Id == DistributorId);

                if (distributor == null)
                {
                    return NotFound("Distributor not found.");
                }

                var products = (from item in unitOfWork.Repository<Entities.Models.Item>().GetAll()
                                join itemType in unitOfWork.Repository<Entities.Models.ItemType>().GetAll()
                                    on item.ItemTypeId equals itemType.Id
                                join subCategory in unitOfWork.Repository<Entities.Models.SubCategory>().GetAll()
                                    on itemType.SubCategoryId equals subCategory.Id
                                join category in unitOfWork.Repository<Entities.Models.Category>().GetAll()
                                    on subCategory.CategoryId equals category.Id
                                join categoryStore in unitOfWork.Repository<ERP.Entities.Models.CategoryStore>().GetAll()
                                    on category.Id equals categoryStore.CategoryId
                                join store in unitOfWork.Repository<Entities.Models.Store>().GetAll()
                                    on categoryStore.StoreId equals store.Id
                                where store.Id == 3 && category.CompanyId == lIntKhilafatCompanyId
                                orderby item.Name
                                select new
                                {
                                    item.Id,
                                    item.Name,
                                    Type = itemType.Name,
                                    VolumeInMl = item.Volume,
                                    item.QuantityInPack,
                                    Image = !string.IsNullOrEmpty(item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                            ? GetImageAsBase64(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                            : null
                                }).ToList();


                Expression<Func<Entities.Models.Order, bool>> predicate = x => x.IsActive == true && x.DealershipId == DistributorId;

                Expression<Func<Entities.Models.Order, object>>[] includes = {
                            x => x.OrderItems,
                            x => x.DispatchOrder,
                        };

                List<string> thenInclude = new List<string>();

                thenInclude.Add("DispatchOrder.DispatchDetail");
                thenInclude.Add("DispatchOrder.DispatchDetail.OrderItem");


                var lObjOrderEntity = unitOfWork.Repository<Entities.Models.Order>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, thenInclude, includes);

                //var _OrderEntity = lObjOrderEntity.Item1.ToList().FirstOrDefault();

                //var dispatchItemWiseSum = _OrderEntity.DispatchOrder
                //       .Where(d => d.IsActive == true && d.StatusId == (long)OrderStatusEnum.OrderReceived) 
                //       .SelectMany(d => d.DispatchDetail)  
                //       .Where(dd => dd.IsActive == true)   
                //       .GroupBy(dd => dd.OrderItem.ItemId)          
                //       .Select(g => new
                //       {
                //           ItemId = g.Key, //OrderItemId                
                //           TotalQuantity = g.Sum(dd => dd.Quantity) 
                //       })
                //       .ToList();

                var dispatchItemWiseSum = lObjOrderEntity.Item1
                                   .SelectMany(order => order.DispatchOrder) // Flatten DispatchOrder across all orders
                                   .Where(d => d.IsActive == true && d.StatusId == (long)OrderStatusEnum.OrderReceived)
                                   .SelectMany(d => d.DispatchDetail) // Flatten DispatchDetail across all DispatchOrders
                                   .Where(dd => dd.IsActive == true)
                                   .GroupBy(dd => dd.OrderItem.ItemId)
                                   .Select(g => new
                                   {
                                       ItemId = g.Key, // OrderItemId                
                                       TotalQuantity = g.Sum(dd => dd.Quantity)
                                   })
                                   .ToList();

                var productList = products.Select(p => new GetDistributorProductStock
                {
                    Id = p.Id,
                    Name = p.Name,
                    HoldQuantity = 0,
                    TransitQuantity = 0,
                    SoldQuantity = 0,
                    LeftQuantity = 0,
                    Type = p.Name,
                    VolumeInMl = p.VolumeInMl,
                    QuantityInPack = p.QuantityInPack,
                    RetailPrice = 0,
                    TradePrice = 0,
                    DistributorPrice = 0,
                    ProductImagePath = p.Image
                }).ToList();


                foreach (var item in productList)
                {
                    var ordersForProduct = dispatchItemWiseSum.FirstOrDefault(g => g.ItemId == item.Id);

                    if (ordersForProduct != null)
                    {
                        item.LeftQuantity = (int)ordersForProduct.TotalQuantity;
                    }
                }

                return this.Result(ResponseStatus.OK, productList, productList.Count().ToString());



                //var dealerOrders = await unitOfWork.Repository<OrderItems>().GetAsync(
                //    oi => (oi.Order.Dealership.TerritoryId == distributor.TerritoryId || oi.Order.Shop.TerritoryId == distributor.TerritoryId) &&
                //          products.Select(p => p.Id).Contains(oi.ItemId),
                //    null,
                //    null,
                //    "Order,Order.Dealership,Order.Shop"
                //);
                //var groupedDealerOrders = dealerOrders.GroupBy(o => o.ItemId);

                //foreach (var item in productList)
                //{
                //    var ordersForProduct = dispatchItemWiseSum.FirstOrDefault(g => g.Key == item.Id);
                //    if (ordersForProduct != null)
                //    {
                //        var dealerStock = ordersForProduct.Where(y =>
                //            y.Order.Dealership != null &&
                //            y.Order.Dealership.TerritoryId == distributor.TerritoryId &&
                //            y.Order.OrderStatusId == (long)OrderStatusEnum.OrderReceived
                //        );

                //        int totalDealerQty = dealerStock.Sum(s => s.ShippedQuantity ?? 0);


                //        var shopOrders = ordersForProduct.Where(y =>
                //            y.Order.Shop != null &&
                //            y.Order.Shop.TerritoryId == distributor.TerritoryId
                //        );
                //        item.HoldQuantity = shopOrders.Where(y => y.Order.OrderStatusId == (long)OrderStatusEnum.OrderConfirm).Sum(s => s.Quantity);
                //        item.TransitQuantity = shopOrders.Where(y => y.Order.OrderStatusId == (long)OrderStatusEnum.OrderDispatched).Sum(s => s.ShippedQuantity ?? 0);
                //        item.SoldQuantity = shopOrders.Where(y => y.Order.OrderStatusId == (long)OrderStatusEnum.OrderReceived).Sum(s => s.ShippedQuantity ?? 0);
                //        item.LeftQuantity = totalDealerQty - item.SoldQuantity;
                //    }
                //}



                //return this.Result(ResponseStatus.OK, productList, productList.Count().ToString());
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        [Route("ReceiveOrderByDOId")]
        public async Task<ActionResult<string>> ReceiveOrderByDOId(ReceiveOrderByDOIdCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                    if (command.AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }

                    var _DispatchOrder = await unitOfWork.Repository<ERP.Entities.Models.DispatchOrder>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && x.Id == command.DOId, null, null, null);

                    if (_DispatchOrder != null)
                    {
                        if (_DispatchOrder.StatusId == (long)OrderStatusEnum.OrderReceived)
                        {
                            return this.Result(ResponseStatus.ValidationFailed, null, "Dispatch Already Received");
                        }

                        if (_DispatchOrder.DCCode != command.DeliveryChallanCode)
                        {
                            return this.Result(ResponseStatus.ValidationFailed, null, "Delivery challan code not matched");
                        }

                        _DispatchOrder.StatusId = (long)OrderStatusEnum.OrderReceived;
                        _DispatchOrder.ReceivedById = new Guid(command.UserId);
                        _DispatchOrder.ReceivedDate = DateTime.Now;
                        _DispatchOrder.ModifiedById = new Guid(command.UserId);
                        _DispatchOrder.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<Entities.Models.DispatchOrder>().Update(_DispatchOrder);

                        OrderProcess process = new OrderProcess();
                        process.FromStatusId = (long)OrderStatusEnum.OrderDispatched;
                        process.ToStatusId = (long)OrderStatusEnum.OrderReceived;
                        process.Comments = "Dispatch " + command.DOId + " Received." + command.Comments;
                        process.CreatedById = new Guid(command.UserId);
                        process.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<Entities.Models.OrderProcess>().Add(process);

                        var check = await unitOfWork.SaveChangesAsync();


                        Expression<Func<Entities.Models.Order, bool>> predicate = x => x.IsActive == true && x.Id == _DispatchOrder.OrderId;

                        Expression<Func<Entities.Models.Order, object>>[] includes = {
                            x => x.OrderItems,
                            x => x.DispatchOrder,
                        };


                        List<string> thenInclude = new List<string>();

                        thenInclude.Add("DispatchOrder.DispatchDetail");


                        var lObjOrderEntity = unitOfWork.Repository<Entities.Models.Order>().GetPagingWhereAsNoTrackingAsync(predicate, null, null, null, thenInclude, includes);

                        var _OrderEntity = lObjOrderEntity.Item1.ToList().FirstOrDefault();

                        var TotalOrderItemQty = _OrderEntity.OrderItems.Where(x => x.IsActive == true).Sum(x => x.Quantity);

                        var DispatchSumItem = _OrderEntity.DispatchOrder.Where(d => d.IsActive == true && d.StatusId == (long)OrderStatusEnum.OrderReceived).SelectMany(d => d.DispatchDetail).Where(dd => dd.IsActive == true).Sum(dd => dd.Quantity);

                        if (TotalOrderItemQty == DispatchSumItem)
                        {
                            var _UpdateOrder = await unitOfWork.Repository<ERP.Entities.Models.Order>().GetFirstAsNoTrackingAsync(x => x.IsActive == true && x.IsDelete == false && x.Id == _OrderEntity.Id, null, null, null);

                            _UpdateOrder.ModifiedById = new Guid(command.UserId);
                            _UpdateOrder.ModifiedDate = DateTime.Now;
                            _UpdateOrder.OrderStatusId = (long)OrderStatusEnum.OrderReceived;
                            unitOfWork.Repository<Entities.Models.Order>().Update(_UpdateOrder);

                            OrderProcess lOrderRecvprocess = new OrderProcess();
                            lOrderRecvprocess.FromStatusId = (long)OrderStatusEnum.OrderDispatched;
                            lOrderRecvprocess.ToStatusId = (long)OrderStatusEnum.OrderReceived;
                            lOrderRecvprocess.Comments = "Order Completly Received";
                            lOrderRecvprocess.CreatedById = new Guid(command.UserId);
                            lOrderRecvprocess.CreatedDate = DateTime.Now;
                            unitOfWork.Repository<Entities.Models.OrderProcess>().Add(lOrderRecvprocess);

                            var OrderSaveCheck = await unitOfWork.SaveChangesAsync();
                        }


                        if (check > 0)
                        {
                            return this.Result(ResponseStatus.OK, command.DOId, "Dispatch Received successfully");
                        }
                        else
                        {

                            return this.Result(ResponseStatus.Error, command.DOId, "Dispatch Received failed");
                        }
                    }
                    else
                    {
                        return this.Result(ResponseStatus.RecordNotFound, null, "Dispatch Not Found");
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("DeleteAccount")]
        public async Task<ActionResult<string>> DeleteAccount(DeleteAccountCommand command)
        {
            try
            {
                if (IsValidToken(Request.Headers.Authorization) == false)
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }
                if (string.IsNullOrWhiteSpace(command.UserId))
                {
                    return this.Result(ResponseStatus.DateNotMatch, null, "User Id Missing");
                }
                var lObjOrderItems = from u in unitOfWork.Repository<Entities.Models.AspNetUsers>().GetAll()
                                     where u.Id == new Guid(command.UserId)
                                     select u;
                if (lObjOrderItems.Count() == 0)
                {
                    return this.Result(ResponseStatus.DateNotMatch, null, "user not found");
                }
                return this.Result(ResponseStatus.OK, null, "User Deleted Successfully");
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.InnerException.Message);
            }


        }

        #endregion

        #region Dealership Endpoints

        [HttpPost]
        [Route("GetDealershipOrder")]
        public async Task<ActionResult<string>> GetDealershipOrder([FromBody] GetDealershipOrderQuery getAllOrderQuery)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                if (!IsValidToken(Request.Headers.Authorization))
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                if (getAllOrderQuery.AppDateTime.Date != DateTime.Now.Date)
                {
                    return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                }

                if (getAllOrderQuery.DealershipId == null || getAllOrderQuery.DealershipId == 0)
                {
                    return this.Result(ResponseStatus.BadRequest, null, "DealershipId is required");
                }

                if (getAllOrderQuery.FDate == null)
                {
                    return this.Result(ResponseStatus.BadRequest, null, "From date is required");
                }

                if (getAllOrderQuery.TDate == null)
                {
                    return this.Result(ResponseStatus.BadRequest, null, "To date is required");
                }

                if (getAllOrderQuery.StatusId == null)
                {
                    return this.Result(ResponseStatus.BadRequest, null, "StatusId is required");
                }

                if (getAllOrderQuery.PagingData == null || getAllOrderQuery.PagingData.Take <= 0)
                {
                    return this.Result(ResponseStatus.DateNotMatch, null, "PagingData (take) should be greater than 0");
                }

                var data = await this.mediator.Send(getAllOrderQuery);

                foreach (var items in data.Item1)
                {
                    
                    foreach (var item in items.OrderAttachments)
                    {
                        item.ImageName = !string.IsNullOrEmpty(item.ImageName) && System.IO.File.Exists(Path.Combine(Localcontainer, item.ImageName.TrimStart('/')))
                             ? GetImageAsBase64(Path.Combine(Localcontainer, item.ImageName.TrimStart('/')))
                             : null;
                    }

                    foreach (var item in items.OrderItems)
                    {
                        item.Item.Image = !string.IsNullOrEmpty(item.Item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, item.Item.Image.TrimStart('/')))
                             ? GetImageAsBase64(Path.Combine(Localcontainer, item.Item.Image.TrimStart('/')))
                             : null;
                    }
                }

                return this.Result(ResponseStatus.OK, data.Item1, "Dealership Orders");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                // Optionally log to file or console here
                return this.Result(ResponseStatus.Error, null, message);
            }
        }


        [HttpPost]
        [Route("GetDealershipAccountLedger")]
        public async Task<ActionResult<string>> GetDealershipAccountLedger([FromBody] GetDealershipAccountLedgerQuery getDealershipAccountLedgerQuery)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                if (!IsValidToken(Request.Headers.Authorization))
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                if (getDealershipAccountLedgerQuery.AppDateTime.Date != DateTime.Now.Date)
                {
                    return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                }

                if (getDealershipAccountLedgerQuery.DealershipId == null || getDealershipAccountLedgerQuery.DealershipId == 0)
                {
                    return this.Result(ResponseStatus.BadRequest, null, "DealershipId is required");
                }

                if (getDealershipAccountLedgerQuery.FDate == null)
                {
                    return this.Result(ResponseStatus.BadRequest, null, "From date is required");
                }

                if (getDealershipAccountLedgerQuery.TDate == null)
                {
                    return this.Result(ResponseStatus.BadRequest, null, "To date is required");
                }

                //if (getDealershipAccountLedgerQuery.StatusId == null)
                //{
                //    return this.Result(ResponseStatus.BadRequest, null, "StatusId is required");
                //}

                //if (getDealershipAccountLedgerQuery.PagingData == null || getDealershipAccountLedgerQuery.PagingData.Take <= 0)
                //{
                //    return this.Result(ResponseStatus.DateNotMatch, null, "PagingData (take) should be greater than 0");
                //}

                var data = await this.mediator.Send(getDealershipAccountLedgerQuery);
                return this.Result(ResponseStatus.OK, data, "Dealership Account Ledger");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                // Optionally log to file or console here
                return this.Result(ResponseStatus.Error, null, message);
            }
        }

        [HttpPost]
        [Route("GetDealershipStockBalance")]
        public async Task<ActionResult<string>> GetDealershipStockBalance([FromBody] GetDealershipStockBalanceQuery getDealershipAccountLedgerQuery)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                if (!IsValidToken(Request.Headers.Authorization))
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                if (getDealershipAccountLedgerQuery.AppDateTime.Date != DateTime.Now.Date)
                {
                    return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                }

                if (getDealershipAccountLedgerQuery.DealershipId == null || getDealershipAccountLedgerQuery.DealershipId == 0)
                {
                    return this.Result(ResponseStatus.BadRequest, null, "DealershipId is required");
                }

                var data = await this.mediator.Send(getDealershipAccountLedgerQuery);
                return this.Result(ResponseStatus.OK, data, "Dealership Stock Balance");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                // Optionally log to file or console here
                return this.Result(ResponseStatus.Error, null, message);
            }
        }

        [HttpPost]
        [Route("GetShopOrderByDealership")]
        public async Task<ActionResult<string>> GetShopOrderByDealership(GetShopOrderByDealershipQuery getAllShopOrderQuery)
        {
            try
            {
                // Validate token
                if (!IsValidToken(Request.Headers.Authorization))
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                // Validate date (allowing for some small tolerance, e.g., a few minutes)
                if (getAllShopOrderQuery.AppDateTime.Value.Date != DateTime.Now.Date)
                {
                    return this.Result(ResponseStatus.DateNotMatch, null, "AppDateTime does not match today's date");
                }

                var data = await this.mediator.Send(getAllShopOrderQuery);
                foreach (var order in data.Item1)
                {
                    foreach (var item in order.ShopOrderItems)
                    {
                        if (!string.IsNullOrEmpty(item.Item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, item.Item.Image.TrimStart('/'))))
                        {
                            item.Item.Image = GetImageAsBase64(Path.Combine(Localcontainer, item.Item.Image.TrimStart('/')));
                        }
                        else
                        {
                            item.Item.Image = null;
                        }
                    }
                }
                return this.Result(ResponseStatus.OK, data.Item1, "Get Shop Order By User");
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetShopOrderByUser")]
        public async Task<ActionResult> GetShopOrderByUser(GetShopOrderByUserQuery getAllShopOrderQuery)
        {
            try
            {
                // Validate token
                if (!IsValidToken(Request.Headers.Authorization))
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                // Validate date (allowing for some small tolerance, e.g., a few minutes)
                if (getAllShopOrderQuery.AppDateTime.Value.Date != DateTime.Now.Date)
                {
                    return this.Result(ResponseStatus.DateNotMatch, null, "AppDateTime does not match today's date");
                }
                var data = await this.mediator.Send(getAllShopOrderQuery);
                foreach (var order in data.Item1)
                {
                    foreach (var item in order.ShopOrderItems)
                    {
                        if (!string.IsNullOrEmpty(item.Item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, item.Item.Image.TrimStart('/'))))
                        {
                            item.Item.Image = GetImageAsBase64(Path.Combine(Localcontainer, item.Item.Image.TrimStart('/')));
                        }
                        else
                        {
                            item.Item.Image = null;
                        }
                    }
                }
                return this.Result(ResponseStatus.OK, data, "Get Shop Order By User");
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        #endregion

        #region Secondary Sales

        [HttpGet]
        [Route("GetShopOrderById")]
        public async Task<ActionResult<GetShopOrder>> GetById(long id)
        {
            try
            {
                // Validate token
                if (!IsValidToken(Request.Headers.Authorization))
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                var data = await this.mediator.Send(new GetShopOrderByIdQuery(id));
                return this.Result(ResponseStatus.OK, data, "Get Shop Order By Id");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                // Optionally log to file or console here
                return this.Result(ResponseStatus.Error, null, message);
            }
        }

        [HttpPost]
        [Route("GetAllShopOrder")]
        public async Task<ActionResult<Tuple<IEnumerable<GetShopOrder>, long>>> GetAll(GetAllShopOrderQuery getAllShopOrderQuery)
        {
            try
            {
                // Validate token
                if (!IsValidToken(Request.Headers.Authorization))
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }
                          
                var data = await this.mediator.Send(getAllShopOrderQuery);
                return this.Result(ResponseStatus.OK, data, "GetAllShopOrder");
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveShopOrder")]
        public async Task<IActionResult> Save(CreateShopOrderCommand command)
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
                        return this.Result(ResponseStatus.OK, "Shop Order Saved!", null);
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

        [HttpGet]
        [Route("DeleteShopOrder")]
        public async Task<ActionResult<long>> DeleteShopOrder(long id)
        {
            try
            {
                var result = await this.mediator.Send(new DeleteShopOrderQuery(id));
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Shop Order is used!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Deleting Shop Order!");
                }
                else if (result == (long)ResponseStatus.OK)
                {
                    return this.Result(ResponseStatus.OK, null, "Successfully Deleted!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Something went Wrong!");
                }
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                // Optionally log to file or console here
                return this.Result(ResponseStatus.Error, null, message);
            }
        }

        [HttpPost]
        [Route("UpdateShopOrderStatus")]
        public async Task<ActionResult<long>> UpdateShopOrderStatus(UpdateShopOrderStatusQuery updateShopOrderStatusQuery)
        {
            try
            {
                var result = await this.mediator.Send(updateShopOrderStatusQuery);
                if (result == (long)ResponseStatus.Conflict)
                {
                    return this.Result(ResponseStatus.Conflict, null, "Conflict!");
                }
                else if (result == (long)ResponseStatus.Error)
                {
                    return this.Result(ResponseStatus.Error, null, "Error Confirming!");
                }
                else if (result == (long)ResponseStatus.OK)
                {
                    return this.Result(ResponseStatus.OK, null, "Confirmed!");
                }
                else
                {
                    return this.Result(ResponseStatus.Error, null, "Something went Wrong!");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("GetShopsByTerritoryPaging")]
        public async Task<ActionResult<string>> GetShopsByTerritoryPaging([FromBody] GetShopsByTerritoryPagingQuery getShopsByTerritoryIdQuery)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }

                if (!IsValidToken(Request.Headers.Authorization))
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                if (getShopsByTerritoryIdQuery.AppDateTime.Date != DateTime.Now.Date)
                {
                    return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                }

                if (getShopsByTerritoryIdQuery.PagingData == null || getShopsByTerritoryIdQuery.PagingData.Take <= 0)
                {
                    return this.Result(ResponseStatus.DateNotMatch, null, "PagingData (take) should be greater than 0");
                }

                var data = await this.mediator.Send(getShopsByTerritoryIdQuery);
                return this.Result(ResponseStatus.OK, data, "Shops By Territory");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                // Optionally log to file or console here
                return this.Result(ResponseStatus.Error, null, message);
            }
        }

        [HttpGet]
        [Route("GetKCItemsByDistributorShop")]
        public async Task<ActionResult<string>> GetKCItemsByDistributorShop(long DistributorId)
        {
            try
            {
                var data = await this.mediator.Send(new GetKCItemsByDistributorShopQuery(DistributorId));
                foreach (var item in data)
                {
                    item.Image = !string.IsNullOrEmpty(item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                         ? GetImageAsBase64(Path.Combine(Localcontainer, item.Image.TrimStart('/')))
                                         : null;
                }

                // Serialize the response in camel case
                return this.Result(ResponseStatus.OK, data, "Get KCItems By Distributor Shop");
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                // Optionally log to file or console here
                return this.Result(ResponseStatus.Error, null, message);
            }
        }

        [HttpPost]
        [Route("SaveShopDispatch")]
        public async Task<IActionResult> Save(SaveShopDispatchCommand command)
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
                        return this.Result(ResponseStatus.OK, "Shop Dispatch Saved!", null);
                    }
                    else if (result == 500)
                    {
                        return this.Result(ResponseStatus.Conflict, "Order Not Found!", null);
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

        [HttpPost]
        [Route("GetPendingShopOrder")]
        public async Task<ActionResult<List<GetShopOrder>>> GetPendingShopOrder(GetPendingShopOrderForDispatchQuery ShopOrderIds)
        {
            try
            {
                var data = await this.mediator.Send(ShopOrderIds);
                foreach (var itemorder in data)
                {
                    foreach (var items in itemorder.ShopOrderItems)
                    {
                        items.Item.Image = !string.IsNullOrEmpty(items.Item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, items.Item.Image.TrimStart('/')))
                                    ? GetImageAsBase64(Path.Combine(Localcontainer, items.Item.Image.TrimStart('/')))
                                    : null;
                    }
                }

                return this.Result(ResponseStatus.OK, data, "GetPendingShopOrder");
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetPendingShopOrderItems")]
        public async Task<ActionResult<List<GetShopOrderItems>>> GetPendingShopOrderItems(long ShopOrderId, long ShopDispatchId,long DealershipId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }

                    var data = await this.mediator.Send(new GetPendingShopOrderItemsForDispatchQuery(ShopOrderId, ShopDispatchId, DealershipId));
                    foreach (var items in data)
                    {
                            items.Item.Image = !string.IsNullOrEmpty(items.Item.Image) && System.IO.File.Exists(Path.Combine(Localcontainer, items.Item.Image.TrimStart('/')))
                                        ? GetImageAsBase64(Path.Combine(Localcontainer, items.Item.Image.TrimStart('/')))
                                        : null;
                    }
                    return this.Result(ResponseStatus.OK, data, "Get Pending Shop Order Items");
                }
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                // Optionally log to file or console here
                return this.Result(ResponseStatus.Error, null, message);
            }
        }
        
        [HttpGet]
        [Route("RejectShopOrder")]
        public async Task<ActionResult<bool>> RejectShopOrder(DateTime AppDateTime, long id, Guid UserId,string Remarks)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.Result(ResponseStatus.Error, null, this.GetModelValidationErrors(this.ModelState));
                }
                else
                {
                    if (IsValidToken(Request.Headers.Authorization) == false)
                    {
                        return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                    }
                 
                    if (AppDateTime.Date != DateTime.Now.Date)
                    {
                        return this.Result(ResponseStatus.DateNotMatch, null, "Date not matched");
                    }

                    var data = await this.mediator.Send(new RejectShopOrderQuery(id, UserId, Remarks));
                    return this.Result(ResponseStatus.OK, data, "Reject Shop Order");
                }
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveShopOrderByDealership")]
        public async Task<ActionResult<bool>> SaveShopOrderByDealership(CreateShopOrderByDealershipCommand command)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    var validationErrors = this.GetModelValidationErrors(this.ModelState);
                    return this.Result(ResponseStatus.Error, null, validationErrors);
                }

                // Validate token
                if (!IsValidToken(Request.Headers.Authorization))
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                // Validate date (allowing for some small tolerance, e.g., a few minutes)
                if (command.AppDateTime.Date != DateTime.Now.Date)
                {
                    return this.Result(ResponseStatus.DateNotMatch, null, "AppDateTime does not match today's date");
                }

                // Handle the command
                var result = await this.mediator.Send(command);
                // Return the result
                return result == 200 ? this.Result(ResponseStatus.OK, true, "Shop Order Saved Successfully") :
                                this.Result(ResponseStatus.Error, null, "Failed to Save Shop Order");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                return this.Result(ResponseStatus.Error, null, "An error occurred while processing your request");
            }
        }

        #endregion

        #region Appoinment

        [HttpGet]
        [Route("GetAppoinmentById")]
        public async Task<ActionResult<GetAppointment>> GetAppoinmentById(long id)
        {
            try
            {
                return await this.mediator.Send(new GetAppoinmentByIdQuery(id));
            }
            catch (Exception ex)
            {
                return this.Result(ResponseStatus.Error, null, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveAppointmentAttachment")]
        public async Task<ActionResult<bool>> SaveAppointmentAttachment(SaveAppointmentAttachmentCommand command)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    var validationErrors = this.GetModelValidationErrors(this.ModelState);
                    return this.Result(ResponseStatus.Error, null, validationErrors);
                }

                // Validate token
                if (!IsValidToken(Request.Headers.Authorization))
                {
                    return this.Result(ResponseStatus.InvalidToken, null, "Authentication Failed");
                }

                // Handle the command
                var result = await this.mediator.Send(command);
                // Return the result
                return result == 200 ? this.Result(ResponseStatus.OK, true, "Appointment Attachment Saved Successfully") :
                                this.Result(ResponseStatus.Error, null, "Failed to Save Appointment Attachment");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                return this.Result(ResponseStatus.Error, null, "An error occurred while processing your request");
            }
        }

        #endregion

    }
}