using System;
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
using ERP.Services.Interfaces;

namespace ERP.Mediator.Mediator.Auth.Handler
{
    public class UpdateSaleHandler : BaseHandler, IRequestHandler<UpdateSaleCommand, IdentityResponse>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AspNetUsersModel> userManager;
        private readonly SessionProvider sessionProvider;

        public UpdateSaleHandler(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AspNetUsersModel> userManager, UpdateValidator validatior,
            SessionProvider sessionProvider, IBlobService blobService)
            : base(sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.sessionProvider = sessionProvider;
        }

        public async Task<IdentityResponse> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var result = new IdentityResponse();
            var user = await userManager.FindByIdAsync(request.Id.ToString());

            if (user == null)
            {
                result.Error = "User not found.";
                return result;
            }

            bool IsRegisterDeviceForMobile = false;

            // Update only the desired fields
            if (user.IsMobileDeviceRegister != true && request.IsMobileDeviceRegister == true)
            {
                IsRegisterDeviceForMobile = true;
            }

            user.DealershipId = request.DealershipId;
            user.IsMobileDeviceRegister = request.IsMobileDeviceRegister;
            user.IsAvailableForMobile = request.IsAvailableForMobile;
            user.IsAvailableForWeb = request.IsAvailableForWeb;
            user.IsDistCompForAtten = request.IsDistCompForAtten;
            user.ModifiedById = sessionProvider.Session.LoggedInUserId;
            user.ModifiedDate = DateTime.Now;
            user.EmployeeWorkSiteTypeId = request.EmployeeWorkSiteTypeId;

            var _user = mapper.Map<AspNetUsers>(user);
            unitOfWork.Repository<AspNetUsers>().Update(_user);
            await unitOfWork.SaveChangesAsync();

            // Handle unregistering other devices
            if (IsRegisterDeviceForMobile)
            {
                var otherUsers = unitOfWork.Repository<AspNetUsers>().GetAll()
                    .Where(u => u.DeviceId == user.DeviceId && u.Email != user.Email && u.IsMobileDeviceRegister == true)
                    .ToList();

                foreach (var item in otherUsers)
                {
                    item.DeviceId = null;
                    item.IsMobileDeviceRegister = false;
                    unitOfWork.Repository<AspNetUsers>().Update(item);
                }

                unitOfWork.SaveChanges();
            }

            return result;
        }

    }
}