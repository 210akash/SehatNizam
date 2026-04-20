using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Notification.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Notification.Handler
{
    public class SaveNotificationHandler : IRequestHandler<SaveNotificationCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveNotificationHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveNotificationCommand, long>.Handle(SaveNotificationCommand request, CancellationToken cancellationToken)
        {
            // Validation: Expire Date should not be in the past
            if (request.ExpireDate.Date < DateTime.Now.Date)
            {
                return 400; // Bad Request - Expire date in past
            }

            // Validation: Required fields
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Message))
            {
                return 400; // Bad Request - Required fields missing
            }

            var notification = await unitOfWork.Repository<Entities.Models.Notification>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            
            // Check for duplicate active notification with same title in same department
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.Notification>().GetAsync(x => x.IsActive == true && x.IsDelete == false && x.Id != request.Id
                                 && x.DepartmentId == request.DepartmentId && x.Title.ToLower() == request.Title.ToLower());

            if (checkDuplicate.Any())
            {
                return 409; // Conflict - Duplicate exists
            }

            if (notification == null)
            {
                // Create new
                var _notification = mapper.Map<Entities.Models.Notification>(request);
                _notification.CreatedById = sessionProvider.Session.LoggedInUserId;
                _notification.CreatedDate = DateTime.Now;
                _notification.IsActive = true;
                _notification.IsDelete = false;
                unitOfWork.Repository<Entities.Models.Notification>().Add(_notification);
                SaveChanges();
            }
            else
            {
                // Update existing
                var _notification = mapper.Map<Entities.Models.Notification>(request);
                _notification.CreatedById = notification.CreatedById;
                _notification.CreatedDate = notification.CreatedDate;
                _notification.ModifiedById = sessionProvider.Session.LoggedInUserId;
                _notification.ModifiedDate = DateTime.Now;
                _notification.IsActive = notification.IsActive;
                _notification.IsDelete = notification.IsDelete;
                unitOfWork.Repository<Entities.Models.Notification>().Update(_notification);
                SaveChanges();
            }
            
            return 200;
        }
    }
}
