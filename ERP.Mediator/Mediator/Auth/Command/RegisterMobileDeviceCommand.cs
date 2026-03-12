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
    public class RegisterMobileDeviceCommand : IRequest<IdentityResponse>
    {
        public Guid Id { get; set; }
        public bool IsAvailableForMobile { get; set; }
        public bool IsAvailableForWeb { get; set; }
        public bool IsMobileDeviceRegister { get; set; }
        public bool IsDistCompForAtten { get; set; }
    }
}
