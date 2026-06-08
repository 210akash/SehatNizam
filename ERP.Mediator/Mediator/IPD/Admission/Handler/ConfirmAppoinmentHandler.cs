using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Appointment.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class ConfirmAppoinmentHandler : IRequestHandler<ConfirmAppoinmentQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public ConfirmAppoinmentHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<Tuple<long, string>> Handle(ConfirmAppoinmentQuery request, CancellationToken cancellationToken)
        {
            int check = 0;

            // Fetch the appointment with status 1
            var appointment = await unitOfWork
                .Repository<Entities.Models.Appointment>()
                .GetFirstAsync(
                    y => y.Id == request.Id && y.AppointmentStatusId == 1,
                    null, null, null);

            if (appointment != null)
            {
                // Update appointment status
                var updateAppointment = unitOfWork.Repository<Entities.Models.Appointment>().GetFirst(y => y.Id == appointment.Id);
                updateAppointment.AppointmentStatusId = 5;
                updateAppointment.ModifiedById = sessionProvider.Session.LoggedInUserId;
                updateAppointment.ModifiedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.Appointment>().Update(updateAppointment);

                // Update related AppointmentPayments
                var patient = unitOfWork.Repository<Entities.Models.Patient>()
                    .Find(x => x.Id == appointment.PatientId);
                if (patient != null && string.IsNullOrEmpty(patient.MRN))
                {
                    var mrn = await GenerateMrnAsync();
                    patient.MRN = mrn; // Set status to 3
                    patient.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    patient.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.Patient>().Update(patient);
                }

                // Update related AppointmentPayments
                var payment = unitOfWork.Repository<AppointmentPayment>()
                    .Find(x => x.AppointmentId == appointment.Id);
                if (payment != null)
                {
                    payment.Discount = request.Discount; // Set status to 3
                    payment.TotalPayable = payment.VisitFee - request.Discount; // Set status to 3
                    payment.PaymentStatusId = 3; // Set status to 3
                    payment.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    payment.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<AppointmentPayment>().Update(payment);
                }

                check = await unitOfWork.SaveChangesAsync();

            }

            if (check > 0)
            {
                return new Tuple<long, string>(200, "Appointment Confirmed Successfully!");
            }
            else
            {
                return new Tuple<long, string>(500, "Error Confirming, Please contact system admin!");
            }
        }

        private async Task<string> GenerateMrnAsync()
        {
            var lastPatient = await unitOfWork.Repository<Entities.Models.Patient>()
                .GetOneAsync(
                    x => !string.IsNullOrEmpty(x.MRN),
                    q => q.OrderByDescending(x => x.Id));

            int next = 1;

            if (lastPatient != null &&
                int.TryParse(lastPatient.MRN, out int lastNo))
            {
                next = lastNo + 1;
            }

            return next.ToString("D6");
        }
    }
}
