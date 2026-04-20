using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Notification.Query
{
    public class GetEmployeeNotificationsQuery : IRequest<IEnumerable<GetNotification>>
    {
    }
}
