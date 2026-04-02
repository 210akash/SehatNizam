using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using global::AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ERP.BusinessModels.BaseVM;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Auth.Command;
using ERP.Mediator.Mediator.Auth.Validator;
using ERP.Repositories.UnitOfWork;
using ERP.BusinessModels.ParameterVM;
using ERP.Services.Implementation;
using ERP.Services.Interfaces;
using ERP.Entities.Migrations;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class UpdateHandler : BaseHandler, IRequestHandler<UpdateCommand, IdentityResponse>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly UpdateValidator validatior;
        private readonly UserManager<AspNetUsersModel> userManager;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public UpdateHandler(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, UpdateValidator validatior,
            SessionProvider sessionProvider, IBlobService blobService)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.validatior = validatior;
            this.userManager = userManager;
            this.sessionProvider = sessionProvider;
            this.blobService = blobService;
        }

        public async Task<IdentityResponse> Handle(UpdateCommand request, CancellationToken cancellationToken)
        {
            bool IsRegisterDeviceForMobile = false;
            //this.validatior.ValidateAndThrow(request);
            var result = new IdentityResponse();

            var checkPhoneNo = unitOfWork.Repository<AspNetUsers>().GetExists(x => x.PhoneNumber == request.PhoneNumber && x.Id != request.Id);
            if (checkPhoneNo == true)
            {
                result.Error = "Phone Number Duplicate!";
                return result;
            }

            var checkEmail = unitOfWork.Repository<AspNetUsers>().GetExists(x => x.Email.ToLower() == request.Email.ToLower() && x.Id != request.Id);
            if (checkEmail == true)
            {
                result.Error = "Email Duplicate!";
                return result;
            }

            var user = await userManager.FindByIdAsync(request.Id.ToString());

            var _user = mapper.Map<AspNetUsersModel>(request);

            _user.CreatedDate = user.CreatedDate;
            _user.CreatedById = user.CreatedById;
            _user.ModifiedById = sessionProvider.Session.LoggedInUserId;
            _user.ModifiedDate = DateTime.Now;
            _user.ConcurrencyStamp = user.ConcurrencyStamp;
            _user.SecurityStamp = user.SecurityStamp;
            _user.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            _user.EmailConfirmed = user.EmailConfirmed;
            _user.PasswordHash = user.PasswordHash;

            if (string.IsNullOrEmpty(user.Code))
            {
                string _EmpCode = "";
                string _EmpPreFix = "";
                var _company = await unitOfWork.Repository<Entities.Models.Company>().GetOneAsync(user => user.Id == sessionProvider.Session.CompanyId, null, null);
                if (_company != null)
                    _EmpPreFix = _company.Code;

                Func<IQueryable<AspNetUsers>, IOrderedQueryable<AspNetUsers>> orderByDesc = query => query.OrderByDescending(x => x.Code);
                var latestUser = await unitOfWork.Repository<AspNetUsers>().GetOneAsync(user => user.IsActive == true && user.Code.StartsWith(_EmpPreFix), orderByDesc, null);

                if (latestUser != null)
                {
                    string numericPart = latestUser.Code.Substring(_EmpPreFix.Length);
                    int latestNumber = int.TryParse(numericPart, out var num) ? num : 0;
                    int newNumber = latestNumber + 1;
                    _EmpCode = newNumber.ToString().PadLeft(5, '0');
                }
                else
                {
                    _EmpCode = "00001";
                }
                _user.Code = _EmpPreFix + _EmpCode;
            }
            else
                _user.Code = user.Code;


            var updateUser = await userManager.UpdateAsync(_user);
            result = this.mapper.Map<IdentityResponse>(updateUser);
            if (result.Succeeded)
            {
                var _userToUpdate = mapper.Map<AspNetUsers>(request);
                _userToUpdate.CreatedDate = user.CreatedDate;
                _userToUpdate.CreatedById = user.CreatedById;
                _userToUpdate.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _userToUpdate.ModifiedDate = DateTime.Now;
                _userToUpdate.ConcurrencyStamp = user.ConcurrencyStamp;
                _userToUpdate.SecurityStamp = user.SecurityStamp;
                _userToUpdate.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                _userToUpdate.EmailConfirmed = user.EmailConfirmed;
                _userToUpdate.PasswordHash = user.PasswordHash;
                _userToUpdate.DealershipId = request.DealershipId;
                _userToUpdate.DeviceId = user.DeviceId;
                _userToUpdate.IsLogedIn = user.IsLogedIn;
                _userToUpdate.Code = _user.Code;
                _userToUpdate.IsMobileDeviceRegister = _user.IsMobileDeviceRegister;
                _userToUpdate.EmployeeWorkSiteTypeId = _user.EmployeeWorkSiteTypeId;

                if (user.IsMobileDeviceRegister != true && _user.IsMobileDeviceRegister == true)
                {
                    IsRegisterDeviceForMobile = true;
                }

                unitOfWork.Repository<AspNetUsers>().Update(_userToUpdate);
                await unitOfWork.SaveChangesAsync();

                var userData = await unitOfWork.Repository<global::ERP.Entities.Models.AspNetUsers>().GetFirstAsync(y => y.Id == request.Id, null, null, "AspNetUserRoles");
                await RemoveExisitingRoles(request.Id);

                foreach (var item in request.RoleId)
                {
                    var userRole = new AspNetUserRoles()
                    {
                        RoleId = item,
                        UserId = user.Id
                    };

                    await SaveAspNetUserRolesAsync(userRole);
                }

                var currentAttach = await unitOfWork.Repository<Attachments>().GetFirstAsNoTrackingAsync(x => x.UserId == user.Id);
                if (currentAttach == null)
                {
                    Attachments attachment = new Attachments();
                    attachment.CreatedDate = DateTime.Now;
                    attachment.CreatedById = sessionProvider.Session.LoggedInUserId;
                    attachment.UserId = _user.Id;

                    BlobImageUploadModel blobModel = new()
                    {
                        File = request.FileSource,
                        FileName = request.ImageName,
                        FolderName = "assets/Files/HR"
                    };

                    attachment.ImageName = "/assets/Files/HR/" + await blobService.UploadBase64FileToBlobAsync(blobModel, request.Extension);
                    await unitOfWork.Repository<Attachments>().AddAsync(attachment);
                }
                else if (!string.IsNullOrWhiteSpace(request.ImageName))
                {
                    currentAttach.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    currentAttach.ModifiedDate = DateTime.Now;

                    BlobImageUploadModel blobModel = new()
                    {
                        File = request.FileSource,
                        FileName = request.ImageName,
                        FolderName = "assets/Files/HR"
                    };

                    currentAttach.ImageName = "/assets/Files/HR/" + await blobService.UploadBase64FileToBlobAsync(blobModel, request.Extension);
                    unitOfWork.Repository<Attachments>().Update(currentAttach);
                }

                if (request.Documents != null)
                {
                    foreach (var item in request.Documents)
                    {
                        if (!string.IsNullOrEmpty(item.FileSource))
                        {
                            var currentEmployeeDoc = await unitOfWork.Repository<EmployeeDocument>().GetFirstAsNoTrackingAsync(x => x.EmployeeId == user.Id && x.EmployeeDocumentTypeId == item.EmployeeDocumentTypeId);
                            if (currentEmployeeDoc == null)
                            {
                                EmployeeDocument attachment = new EmployeeDocument();
                                attachment.CreatedDate = DateTime.Now;
                                attachment.CreatedById = sessionProvider.Session.LoggedInUserId;
                                attachment.EmployeeId = _user.Id;
                                attachment.EmployeeDocumentTypeId = (long)item.EmployeeDocumentTypeId;

                                BlobImageUploadModel blobModel = new()
                                {
                                    File = item.FileSource,
                                    FileName = item.ImageName,
                                    FolderName = "assets/Files/HR"
                                };

                                attachment.Name = "/assets/Files/HR/" + await blobService.UploadBase64FileToBlobAsync(blobModel, item.Extension);
                                await unitOfWork.Repository<EmployeeDocument>().AddAsync(attachment);
                            }
                            else if (!string.IsNullOrWhiteSpace(item.FileSource) && item.FileSource.Contains("base64"))
                            {
                                currentEmployeeDoc.ModifiedById = sessionProvider.Session.LoggedInUserId;
                                currentEmployeeDoc.ModifiedDate = DateTime.Now;

                                BlobImageUploadModel blobModel = new()
                                {
                                    File = item.FileSource,
                                    FileName = item.ImageName,
                                    FolderName = "assets/Files/HR"
                                };

                                currentEmployeeDoc.Name = "/assets/Files/HR/" + await blobService.UploadBase64FileToBlobAsync(blobModel, request.Extension);
                                unitOfWork.Repository<EmployeeDocument>().Update(currentEmployeeDoc);
                            }
                        }
                    }
                }

                if (request.Days != null)
                {
                    var workingDays = await unitOfWork.Repository<EmployeeWorkingDays>().GetFirstAsNoTrackingAsync(x => x.EmployeeId == user.Id);
                    if (workingDays == null)
                    {
                        var map = mapper.Map<EmployeeWorkingDays>(request.Days);
                        map.EmployeeId = user.Id;
                        map.CreatedById = sessionProvider.Session.LoggedInUserId;
                        map.CreatedDate = DateTime.Now;
                        unitOfWork.Repository<EmployeeWorkingDays>().Add(map);
                    }
                    else
                    {
                        var map = mapper.Map(request.Days, workingDays);
                        map.EmployeeId = user.Id;
                        map.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        map.ModifiedDate = DateTime.Now;
                        map.CreatedById = workingDays.CreatedById;
                        map.CreatedDate = workingDays.CreatedDate;
                        unitOfWork.Repository<EmployeeWorkingDays>().Update(map);
                    }
                }

                #region User Warehouse

                    var UserProjectList = await unitOfWork.Repository<Entities.Models.UserProject>()
                        .GetPagingWhereAsNoTrackingAsync(y => y.UserId == request.Id && y.IsActive == true,
                        null, null, null, null, null).Item1.ToListAsync();

                    List<long> previousUserProjectIds = UserProjectList
                        .Select(y => y.ProjectId)
                        .ToList();

                    List<long> currentUserProjectIds = request.ProjectIds;
                    List<long> deletedUserProjectIds = previousUserProjectIds.Except(currentUserProjectIds).ToList();
                    List<long> addUserProjectIds = currentUserProjectIds.Except(previousUserProjectIds).ToList();

                    // Handle deletions
                    foreach (var deletedUserProjectId in deletedUserProjectIds)
                    {
                        Entities.Models.UserProject UserProject = UserProjectList.Where(y => y.ProjectId == deletedUserProjectId).FirstOrDefault();

                        if (UserProject != null)
                        {
                            UserProject.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            UserProject.ModifiedDate = DateTime.Now;
                            UserProject.IsActive = false; // Soft delete
                            UserProject.IsDelete = true; // Soft delete
                            unitOfWork.Repository<Entities.Models.UserProject>().Update(UserProject);
                        }
                    }

                    // Handle additions
                    foreach (var ProjectId in addUserProjectIds)
                    {
                        Entities.Models.UserProject lObjUserProject = new()
                        {
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now,
                            ProjectId = ProjectId,
                            UserId = request.Id
                        };
                        unitOfWork.Repository<Entities.Models.UserProject>().Add(lObjUserProject);
                    }

                #endregion

                await unitOfWork.SaveChangesAsync();
            }

            if (IsRegisterDeviceForMobile == true)
            {
                //Un register Other Devices
                var lObjOtherUserMobileRegister = from u in unitOfWork.Repository<AspNetUsers>().GetAll()
                                                  where u.DeviceId == user.DeviceId && u.Email != user.Email && u.IsMobileDeviceRegister == true
                                                  select u;

                if (lObjOtherUserMobileRegister.Count() > 0)
                {
                    foreach (var item in lObjOtherUserMobileRegister)
                    {
                        item.DeviceId = null;
                        item.IsMobileDeviceRegister = false;
                        unitOfWork.Repository<Entities.Models.AspNetUsers>().Update(item);
                    }
                    unitOfWork.SaveChanges();
                }
            }
            return result;
        }

        private async Task SaveAspNetUserRolesAsync(AspNetUserRoles model)
        {
            await unitOfWork.Repository<AspNetUserRoles>().AddAsync(model);
            await unitOfWork.SaveChangesAsync();
        }

        private async Task<IEnumerable<AspNetUserRoles>> RemoveExisitingRoles(Guid? Id)
        {
            var role = await this.unitOfWork.Repository<AspNetUserRoles>().GetAsync(x => x.UserId == Id);
            foreach (var i in role)
            {
                unitOfWork.Repository<AspNetUserRoles>().Remove(i);
                await this.unitOfWork.SaveChangesAsync();
            }
            return role;
        }
    }
}