using MediatR;
using System;

namespace ERP.Mediator.Mediator.Notification.Command
{
    public class SaveNotificationCommand : IRequest<long>
    {
        public long Id { get; set; }
        public long? DepartmentId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
