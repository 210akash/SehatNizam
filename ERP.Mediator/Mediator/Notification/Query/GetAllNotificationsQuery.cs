using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Notification.Query
{
    public class GetAllNotificationsQuery : IRequest<Tuple<IEnumerable<GetNotification>, long>>
    {
        public long? DepartmentId { get; set; }
        public PagingData PagingData { get; set; }
    }
}
