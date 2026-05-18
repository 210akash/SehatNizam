using System;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Appointment.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class CancelAppoinmentHandler : IRequestHandler<CancelAppoinmentQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public CancelAppoinmentHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, string>> Handle(CancelAppoinmentQuery request, CancellationToken cancellationToken)
        {
            int check = 0;
            var Dispatch = await unitOfWork
                .Repository<Entities.Models.Appointment>()
                .GetFirstAsync(
                  y => y.Id == request.Id && y.AppointmentStatusId == 1,
                    null, null, null);
                
            if (Dispatch != null)
            {
                var updateDispatch = unitOfWork.Repository<Entities.Models.Appointment>().GetFirst(y => y.Id == Dispatch.Id);
                updateDispatch.AppointmentStatusId = 20;
                updateDispatch.ModifiedById = sessionProvider.Session.LoggedInUserId;
                updateDispatch.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Appointment>().Update(updateDispatch);
                check = await unitOfWork.SaveChangesAsync();
            }

            if (check > 0)
            {
                return new Tuple<long, string>(200, "Appointment Cancelled Successfully!");
            }
            else
            {
                return new Tuple<long, string>(500, "Error Cancelling, Please contact system admin!");
            }

        }
    }
}
