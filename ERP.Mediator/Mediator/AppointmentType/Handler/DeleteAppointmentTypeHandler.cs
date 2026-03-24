using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.AppointmentType.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.AppointmentType.Handler
{
    public class DeleteAppointmentTypeHandler : IRequestHandler<DeleteAppointmentTypeQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteAppointmentTypeHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteAppointmentTypeQuery request, CancellationToken cancellationToken)
        {
            var AppointmentType = await unitOfWork.Repository<Entities.Models.AppointmentType>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            AppointmentType.IsDelete = true;
            AppointmentType.IsActive = false;
            AppointmentType.DeleteDate = DateTime.Now;
            AppointmentType.ModifiedDate = DateTime.Now;
            AppointmentType.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.AppointmentType>().Update(AppointmentType);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
