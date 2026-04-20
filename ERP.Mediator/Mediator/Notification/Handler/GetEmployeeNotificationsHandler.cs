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
    public class GetEmployeeNotificationsHandler : IRequestHandler<GetEmployeeNotificationsQuery, IEnumerable<GetNotification>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public GetEmployeeNotificationsHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<IEnumerable<GetNotification>> Handle(GetEmployeeNotificationsQuery request, CancellationToken cancellationToken)
        {
            var employeeDepartmentId = this.sessionProvider.Session.DepartmentId;

            Expression<Func<Entities.Models.Notification, bool>> predicate;

            Expression<Func<Entities.Models.Notification, object>>[] includes = {
                x => x.Department,
                x => x.CreatedBy
            };

            Expression<Func<Entities.Models.Notification, object>> OrderByDesc = x => x.CreatedDate;

            // Get notifications that:
            // 1. Are active (not deleted)
            // 2. Not expired (ExpireDate > Now)
            // 3. Either DepartmentId is NULL (global) OR matches employee's department
            predicate = x => x.IsActive == true 
                && x.IsDelete == false
                && x.ExpireDate > DateTime.Now
                && (x.DepartmentId == null || x.DepartmentId == employeeDepartmentId);

            var notifications =  unitOfWork.Repository<Entities.Models.Notification>()
                .GetPagingWhereAsNoTrackingAsync(predicate, null,null, OrderByDesc, null, includes);

            var result = mapper.Map<IEnumerable<GetNotification>>(notifications.Item1).ToList();
            // Order by created date descending (newest first)
            return result;
        }
    }
}
