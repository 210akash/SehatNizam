using MediatR;
using ERP.BusinessModels.BaseVM;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Mediator.Mediator.User.Command
{
    public class SendInvitationReminderCommand : IRequest<DateTime>
    {
        public Guid Id { get; set; }
    }
}
