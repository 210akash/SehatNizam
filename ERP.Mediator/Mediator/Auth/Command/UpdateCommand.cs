using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;

namespace ERP.Mediator.Mediator.Auth.Command
{   /// <summary>
    /// Declaration of Register Model class.
    /// </summary>
    public class UpdateCommand : IRequest<IdentityResponse>
    {
        /// <summary>
        /// Gets or sets of user name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets of first name
        /// </summary>
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(500, ErrorMessage = "First Name must be between 3 and 500 characters", MinimumLength = 3)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets of last name
        /// </summary>
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(500, ErrorMessage = "Last Name must be between 3 and 500 characters", MinimumLength = 3)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets of email
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets of password
        /// </summary>
        //[Required(ErrorMessage = "Password is required")]
        //[StringLength(50, ErrorMessage = "Password must be between 5 and 50 characters", MinimumLength = 5)]
        //[DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the company identifier.
        /// </summary>
        /// <value>
        /// The company identifier.
        /// </value>
        public string CompanyName { get; set; }
        public long? DepartmentId { get; set; }
        public long? StoreId { get; set; }
        
        public int? IndustryId { get; set; }
        public string Title { get; set; }
        public Guid[]? RoleId { get; set; }
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
