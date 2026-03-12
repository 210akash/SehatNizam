using ERP.BusinessModels.ResponseVM;
using ERP.BusinessModels.ResponseVM.AppVM;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.App.Command
{
    public class LoginCommand : IRequest<AppUserVM>
    {
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "AppDateTime is required")]
        public DateTime AppDateTime { get; set; }
        [Required(ErrorMessage = "DeviceId is required")]
        public string DeviceId { get; set; }
    }
}
