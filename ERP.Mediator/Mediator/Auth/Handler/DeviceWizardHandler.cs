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

namespace ERP.Mediator.Mediator.Auth.Handler
{
    /// <summary>
    /// 
    /// </summary>
    public class DeviceWizardHandler : BaseHandler, IRequestHandler<DeviceWizardCommand, bool>
    {

        /// <summary>
        /// Unit of work Declaration
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckPasswordHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="sessionProvider">The session provider.</param>
        public DeviceWizardHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
            : base(sessionProvider)
        {
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<bool> Handle(DeviceWizardCommand request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.Repository<AspNetUsers>().FindAsync(s => s.Id == SessionProvider.Session.LoggedInUserId);
            user.IsDeviceWizardComplete = request.IsCompleted;
            await unitOfWork.SaveChangesAsync();

            return true;

        }
    }
}