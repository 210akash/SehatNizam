using MediatR;
using ERP.BusinessModels.ResponseVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ERP.Mediator.Mediator.User.Command
{
    public class ConfirmRegisterEmailCommand : IRequest<ConfirmRegisterEmailResponse>
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
