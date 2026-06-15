using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.AdvancePayments.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Twilio.Http;

namespace ERP.Mediator.Mediator.AdvancePayments.Handler
{
    public class SaveAdvancePaymentsHandler : IRequestHandler<SaveAdvancePaymentsCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAdvancePaymentsHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveAdvancePaymentsCommand, long>.Handle(SaveAdvancePaymentsCommand request, CancellationToken cancellationToken)
        {
            var AppointmentId = await unitOfWork.Repository<Entities.Models.Appointment>().GetFirstAsNoTrackingAsync(x => x.Id == request.AppointmentId && x.IsActive == true, null, null, null);

            if (AppointmentId == null)
            {
                return 404;
            }

            var payment = mapper.Map<AdvancePayment>(request);
            payment.CreatedById = sessionProvider.Session.LoggedInUserId;
            payment.CreatedDate = DateTime.Now;
            payment.PaymentDate = DateTime.Now;
            await unitOfWork.Repository<AdvancePayment>().AddAsync(payment);
            SaveChanges();
            return 200;
        }
    }
}