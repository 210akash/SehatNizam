using MediatR;
using ERP.BusinessModels.BaseVM;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Mediator.Mediator.User.Command
{
    public class UpdateUserRoleCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
