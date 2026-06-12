using ERP.Core.Provider;
using ERP.Mediator.Mediator.AppointmentPayments.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.AppointmentPayments.Handler
{
    public class SaveAppointmentPaymentHandler : IRequestHandler<SaveAppointmentPaymentCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAppointmentPaymentHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SaveAppointmentPaymentCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                return 400;
            }

            var payment = await unitOfWork.Repository<Entities.Models.AppointmentPayment>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id && x.IsActive && !x.IsDelete);

            if (payment == null)
            {
                return 404;
            }

            if (payment.PaymentStatusId == 3)
            {
                return 409;
            }

            if (request.Discount > request.VisitFee)
            {
                return 422;
            }

            payment.Discount = request.Discount;
            payment.TotalPayable = request.VisitFee - request.Discount;
            payment.PaymentModeId = request.PaymentModeId;
            payment.ModifiedById = sessionProvider.Session.LoggedInUserId;
            payment.ModifiedDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.AppointmentPayment>().Update(payment);
            await unitOfWork.SaveChangesAsync();

            return 200;
        }
    }
}
