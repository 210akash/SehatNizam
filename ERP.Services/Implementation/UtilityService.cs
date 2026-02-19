//-----------------------------------------------------------------------
// <copyright file="EmailService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Implementation
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ERP.BusinessModels.Enums;
    using ERP.BusinessModels.ParameterVM;
    using ERP.Services.Helper;
    using ERP.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using ERP.Services.Settings;
    using System.Linq;
    using System.Threading;
    using ERP.Entities.Models;
    using ERP.Repositories.UnitOfWork;
    using System;
    using Microsoft.EntityFrameworkCore;
    using ERP.BusinessModels.ResponseVM;
    using Microsoft.AspNetCore.SignalR;
    using ERP.Services.Hubs;
    using ERP.Services.Interfaces;

    /// <summary>
    /// Email service for sending and receiving email
    /// </summary>
    public class UtilityService : IUtilityService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ISmsService smsService;
        private readonly IEmailService emailService;
        private readonly IHubContext<AssetNotificationHub> _assetNotificationHubContext;

        public UtilityService(IUnitOfWork unitOfWork, IHubContext<AssetNotificationHub> assetNotificationHubContext, ISmsService smsService, IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.smsService = smsService;
            this.emailService = emailService;
            this._assetNotificationHubContext = assetNotificationHubContext;
        }

        public async Task UserSendNotification(Guid userId, string Description, int notificationTypeId, List<EmailModelButtons> templateModel, string emailTemplateName = "CommonEmailTemplate.cshtml")
        {
            var userNotification = (from user in this.unitOfWork.Repository<AspNetUsers>().Set
                                    select new
                                    {
                                        Email = user.NormalizedEmail,
                                        UserName = string.Concat( user.FirstName, " ", user.LastName),
                                        PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                                        EmailConfirmed = user.EmailConfirmed,
                                        PhoneNumber = user.PhoneNumber,
                                        IsSMS = true,
                                        IsCall = true,
                                        IsEmail = true ,
                                        //CompanyId = user.CompanyId
                                    }).FirstOrDefault();
            if (userNotification != null)
            {
                templateModel.Add(new EmailModelButtons() { BtnName =  userNotification.UserName, BtnLink = Description });
                if (userNotification.IsSMS && userNotification.PhoneNumberConfirmed)
                {
                    //Thread threadSMS = new Thread(async () =>
                    //await SendSMSToUser(userNotification.PhoneNumber, Description, userId, Convert.ToInt32(userNotification.CompanyId));
                    if (userNotification.IsEmail && userNotification.EmailConfirmed)
                    {
                        //Thread threadSMS = new Thread(async () => 
                        //await SendEmailToUser(userNotification.Email, Description, templateModel, emailTemplateName);

                        await this.emailService.SendAsync(userNotification.Email,
                                Description,
                                Description, templateModel, emailTemplateName);
                        //threadSMS.Start();
                    }
                    var twilioRequestModel = new TwilioRequestModel()
                    {
                        ToNumber = userNotification.PhoneNumber,
                        Message = Description
                    };
                    var IsSentSMS = await this.smsService.SendSMSAsync(twilioRequestModel.Message, twilioRequestModel.ToNumber);
                    if (IsSentSMS == true)
                    {
                        //var model = new SmsHistory()
                        //{
                        //    UserId = userId,
                        //    LastModifiedBy = userId,
                        //    CompanyId = Convert.ToInt32(userNotification.CompanyId),
                        //    Description = Description,
                        //    DateCreated = DateTime.UtcNow,
                        //    CreatedBy = userId,
                        //    DateModified = DateTime.UtcNow
                        //};
                        //this.unitOfWork.Repository<SmsHistory>().Add(model);
                        //this.unitOfWork.SaveChanges();
                    }
                    //;
                    //threadSMS.Start();
                }
                
                if (userNotification.IsCall && userNotification.PhoneNumberConfirmed)
                {
                    //Thread threadSMS = new Thread(async () => 
                    //await SendCallToUser(userNotification.PhoneNumber, Description, userId, Convert.ToInt32(userNotification.CompanyId));
                    var twilioRequestModel = new TwilioRequestModel()
                    {
                        ToNumber = userNotification.PhoneNumber,
                        Message = Description,
                    };
                    var IsCall = await smsService.SendVoiceMessageAsync(twilioRequestModel.Message, twilioRequestModel.ToNumber);
                    if (IsCall)
                    {
                        //var model = new CallHistory()
                        //{
                        //    UserId = userId,
                        //    LastModifiedBy = userId,
                        //    CompanyId = Convert.ToInt32(userNotification.CompanyId),
                        //    Description = Description,
                        //    DateCreated = DateTime.UtcNow,
                        //    CreatedBy = userId,
                        //    DateModified = DateTime.UtcNow
                        //};
                        //this.unitOfWork.Repository<CallHistory>().Add(model);
                        //this.unitOfWork.SaveChanges();
                    }
                    //threadSMS.Start();
                }
            }
        }
        async Task SendSMSToUser(string PhoneNumber, string Description, Guid UserId, int CompanyId)
        {
            var twilioRequestModel = new TwilioRequestModel()
            {
                ToNumber = PhoneNumber,
                Message = Description
            };
            var IsSentSMS = await this.smsService.SendSMSAsync(twilioRequestModel.Message, twilioRequestModel.ToNumber);
            
        }

        async Task SendEmailToUser(string Email, string description, List<EmailModelButtons> templateModel, string templateName)
        {
            this.emailService.SendAsync(Email,
                            description,
                            description, templateModel, templateName);

        }

        async Task SendCallToUser(string PhoneNumber, string Description, Guid UserId, int CompanyId)
        {
            var twilioRequestModel = new TwilioRequestModel()
            {
                ToNumber = PhoneNumber,
                Message = Description,
            };
            var IsCall = await smsService.SendVoiceMessageAsync(twilioRequestModel.Message, twilioRequestModel.ToNumber);
            if (IsCall)
            {
            }
        }
    }
}