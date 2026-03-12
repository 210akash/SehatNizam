//-----------------------------------------------------------------------
// <copyright file="LoginCommand.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------


    using System.ComponentModel.DataAnnotations;
    using MediatR;
    using ERP.BusinessModels.ResponseVM;

namespace ERP.Mediator.Mediator.Auth.Command
{
    /// <summary>
    /// Declaration of Login Command class.
    /// </summary>
    public class DeviceWizardCommand : IRequest<bool>
    {
        public bool IsCompleted { get; set; }
    }
}