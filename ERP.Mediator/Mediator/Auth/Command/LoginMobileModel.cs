using System.ComponentModel.DataAnnotations;

namespace ERP.Mediator.Mediator.Auth.Command
{
    public class LoginMobileModel
    {
        /// <summary>
        /// Gets or sets of email
        /// </summary>
        [Required(ErrorMessage = "Cnic is required")]
        public string Cnic { get; set; }

        /// <summary>
        /// Gets or sets of password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Must be between 5 and 50 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
