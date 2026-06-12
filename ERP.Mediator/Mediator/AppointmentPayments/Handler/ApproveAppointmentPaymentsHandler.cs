using ERP.Core.Provider;
using ERP.Mediator.Mediator.AppointmentPayments.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.AppointmentPayments.Handler
{
    public class ApproveAppointmentPaymentsHandler : IRequestHandler<ApproveAppointmentPaymentsCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public ApproveAppointmentPaymentsHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(ApproveAppointmentPaymentsCommand request, CancellationToken cancellationToken)
        {
            if (request.Payments == null || !request.Payments.Any())
            {
                return 400;
            }

            var paymentIds = request.Payments.Select(x => x.Id).ToList();
            var payments = await unitOfWork.Repository<Entities.Models.AppointmentPayment>()
                .FindAllAsync(x => x.AppointmentId == request.AppointmentId
                    && paymentIds.Contains(x.Id)
                    && x.IsActive
                    && !x.IsDelete
                    && (x.PaymentStatusId == 1 || x.PaymentStatusId == 2));

            if (payments == null || payments.Count != request.Payments.Count)
            {
                return 404;
            }

            var now = DateTime.Now;
            foreach (var item in request.Payments)
            {
                var payment = payments.First(x => x.Id == item.Id);

                if (item.Discount < 0 || item.Discount > payment.VisitFee)
                {
                    return 422;
                }

                payment.Discount = item.Discount;
                payment.TotalPayable = payment.VisitFee - item.Discount;
                payment.PaymentStatusId = 3;
                payment.PaymentModeId = request.PaymentModeId;
                payment.PaymentDate = now;
                payment.ModifiedById = sessionProvider.Session.LoggedInUserId;
                payment.ModifiedDate = now;
                unitOfWork.Repository<Entities.Models.AppointmentPayment>().Update(payment);
            }

            await unitOfWork.SaveChangesAsync();
            return 200;
        }
    }
}
