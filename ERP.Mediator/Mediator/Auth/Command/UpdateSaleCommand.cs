using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using ERP.BusinessModels.ParameterVM;

namespace ERP.Mediator.Mediator.Auth.Command
{   /// <summary>
    /// Declaration of Register Model class.
    /// </summary>
    public class UpdateSaleCommand : IRequest<IdentityResponse>
    {
        public Guid Id { get; set; }

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

        // Fields for KC Users (SALE) START
        public long? DealershipId { get; set; }
        public bool? IsMobileDeviceRegister { get; set; }
        public bool? IsAvailableForMobile { get; set; }
        public bool? IsAvailableForWeb { get; set; }
        public bool? IsDistCompForAtten { get; set; }
        public long? EmployeeWorkSiteTypeId { get; set; }
        // Fields for KC Users (SALE) END
    }
}
