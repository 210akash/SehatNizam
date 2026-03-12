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
    public class Send2FAOTPCommand : IRequest<bool>
    {
        public string PhonoNo { get; set; }
        public string Email { get; set; }
        public bool IsTokenSendToEmail { get; set; }
        public bool IsTokenSendToPhonoNo { get; set; }
    }
}