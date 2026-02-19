using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Mediator.Mediator.User.Command
{
    public class ActivateUserCommand : IRequest<bool>
    {
        public Guid? UserId { get; set; }
    }
}
