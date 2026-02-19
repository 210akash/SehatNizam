using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.User.Command
{
   public class CancelInvitationCommand : IRequest<bool>
    {
        public Guid InvitationId { get; set; }
    }
}
