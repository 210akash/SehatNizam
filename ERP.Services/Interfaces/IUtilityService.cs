//-----------------------------------------------------------------------
// <copyright file="IEmailService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using ERP.BusinessModels.ParameterVM;
    using ERP.BusinessModels.ResponseVM;
    using ERP.Entities.Models;
    using ERP.Services.Hubs;

    /// <summary>
    ///  Email Service Interface
    /// </summary>
    public interface IUtilityService
    {
        Task UserSendNotification(Guid userId, string Description, int notificationTypeId, List<EmailModelButtons> templateModel, string emailTemplateName = "CommonEmailTemplate.cshtml");
       // string GenerateKey();
    }
}
