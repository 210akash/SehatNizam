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
    public class VerifyOTPCommand : IRequest<TokenVM>
    {
        /// <summary>
        /// Gets or sets of email
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        public string Token { get; set; }
        public bool IsRemember { get; set; }
    }
}