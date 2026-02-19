using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Mediator.Mediator.User.Command
{
    public class DeactivateUserCommand : IRequest<bool>
    {
        public string UserId { get; set; }
    }
}
