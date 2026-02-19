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
    public class LoginCommand : IRequest<TokenVM>
    {
        /// <summary>
        /// Gets or sets of email
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets of password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Must be between 5 and 50 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsRemember { get; set; }
        public bool IsPasswordHash { get; set; }
    }
}