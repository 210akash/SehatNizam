using MediatR;
using ERP.BusinessModels.BaseVM;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Mediator.Mediator.User.Command
{
    public class TransferOwnershipCommand : IRequest<bool>
    {
        public Guid TransferToUserId { get; set; }
        public Guid NewRoleId { get; set; }
    }
}
