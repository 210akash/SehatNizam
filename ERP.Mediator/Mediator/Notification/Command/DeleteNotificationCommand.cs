using MediatR;

namespace ERP.Mediator.Mediator.Notification.Command
{
    public class DeleteNotificationCommand : IRequest<bool>
    {
        public long Id { get; set; }

        public DeleteNotificationCommand(long id)
        {
            Id = id;
        }
    }
}
