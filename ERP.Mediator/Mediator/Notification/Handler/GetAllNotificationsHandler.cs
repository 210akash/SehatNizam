using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Notification.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Notification.Handler
{
    public class GetAllNotificationsHandler : IRequestHandler<GetAllNotificationsQuery, Tuple<IEnumerable<GetNotification>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetAllNotificationsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<IEnumerable<GetNotification>, long>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Notification, bool>> predicate;

            Expression<Func<Entities.Models.Notification, object>>[] includes = {
                x => x.Department,
                x => x.CreatedBy
            };

            // Filter by department if specified, otherwise show all for company
            predicate = x => x.IsActive == true && x.IsDelete == false
                && (request.DepartmentId == null || x.DepartmentId == request.DepartmentId);

            Expression<Func<Entities.Models.Notification, object>> OrderByDesc = x => x.CreatedDate;
            
            var entity = unitOfWork.Repository<Entities.Models.Notification>().GetPagingWhereAsNoTrackingAsync(
                predicate, request.PagingData, null, OrderByDesc, null, includes);
            
            var notifications = mapper.Map<IEnumerable<GetNotification>>(entity.Item1.ToList());
            
            // Calculate IsExpired for each notification
            foreach (var notification in notifications)
            {
                notification.IsExpired = DateTime.Now > notification.ExpireDate;
            }
            
            return new Tuple<IEnumerable<GetNotification>, long>(notifications, entity.Item2);
        }
    }
}
