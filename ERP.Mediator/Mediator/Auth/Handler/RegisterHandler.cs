using System;
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
using ERP.Services.Interfaces;
using System.Net;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ERP.Entities.Migrations;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class RegisterHandler : BaseHandler, IRequestHandler<RegisterCommand, IdentityResponse>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly RegisterValidator validatior;
        private readonly UserManager<AspNetUsersModel> userManager;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public RegisterHandler(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, RegisterValidator validatior, SessionProvider sessionProvider, IBlobService blobService)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.validatior = validatior;
            this.userManager = userManager;
            this.sessionProvider = sessionProvider;
            this.blobService = blobService;
        }

        public async Task<IdentityResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            //validatior.ValidateAndThrow(request);
            var result = new IdentityResponse();

            var checkPhoneNo = unitOfWork.Repository<AspNetUsers>().GetExists(x => x.PhoneNumber == request.PhoneNumber);
            if (checkPhoneNo == true)
            {
                result.Error = "Phone Number Duplicate!";
                return result;
            }

            var checkEmail = unitOfWork.Repository<AspNetUsers>().GetExists(x => x.Email.ToLower() == request.Email.ToLower());
            if (checkEmail == true)
            {
                result.Error = "Email Duplicate!";
                return result;
            }

            var _user = mapper.Map<AspNetUsersModel>(request);
            _user.Id = Guid.NewGuid();
            _user.IsActive = true;
            _user.IsDelete = false;
            _user.CreatedById = sessionProvider.Session.LoggedInUserId;
            _user.CreatedDate = DateTime.Now;
            _user.PhoneNumberConfirmed = true;
            _user.EmailConfirmed = true;
            _user.UserName = request.Email.ToLower();
            _user.NormalizedUserName = request.Email;
            _user.ConcurrencyStamp = Guid.NewGuid().ToString();

            string _EmpCode = "";
            string _EmpPreFix = "";
            var _company = await unitOfWork.Repository<Entities.Models.Company>().GetOneAsync(user => user.Id == sessionProvider.Session.CompanyId, null, null);
            if(_company != null)
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
            var savedUser = await userManager.CreateAsync(_user, request.Password);
            result = mapper.Map<IdentityResponse>(savedUser);
            if (result.Succeeded)
            {
                foreach (var item in request.RoleId)
                {
                    var userRole = new AspNetUserRoles()
                    {
                        RoleId = new Guid(item != null ? item : item),
                        UserId = _user.Id
                    };

                    await SaveAspNetUserRolesAsync(userRole);
                }

                if (!string.IsNullOrEmpty(request.FileSource))
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
                    await unitOfWork.SaveChangesAsync();
                }

                if (request.Documents != null)
                {
                    foreach (var item in request.Documents)
                    {
                        if (!string.IsNullOrEmpty(item.FileSource))
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
                    }
                }

                if(request.Days != null)
                {
                    var map = mapper.Map<EmployeeWorkingDays>(request.Days);
                    map.EmployeeId = _user.Id;
                    map.CreatedById = sessionProvider.Session.LoggedInUserId;
                    map.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<EmployeeWorkingDays>().Add(map);
                }

                if(request.ProjectIds !=  null  && request.ProjectIds.Count > 0)
                {
                    foreach (var item in request.ProjectIds)
                    {
                        Entities.Models.UserProject lObjUserProject = new()
                        {
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            CreatedDate = DateTime.Now,
                            UserId = _user.Id
                        };
                        unitOfWork.Repository<Entities.Models.UserProject>().Add(lObjUserProject);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the name of the role by.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>the asp net roles</returns>
        private async Task<AspNetRoles> GetRoleByNameAsync(string roleName)
        {
            var role = await unitOfWork.Repository<AspNetRoles>().FindAsync(x => x.Name == roleName);
            return role;
        }

        /// <summary>
        /// Saves the ASP net user roles.
        /// </summary>
        /// <param name="model">The Asp Net User Roles model.</param>
        /// <returns>the task</returns>
        private async Task SaveAspNetUserRolesAsync(AspNetUserRoles model)
        {
            await unitOfWork.Repository<AspNetUserRoles>().AddAsync(model);
            await unitOfWork.SaveChangesAsync();
        }
    }
}