using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Appointment.Query;
using ERP.Mediator.Mediator.Handler;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class ConfirmAppoinmentHandler : IRequestHandler<ConfirmAppoinmentQuery, Tuple<long, string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMediator mediator;
        private readonly SessionProvider sessionProvider;
        private readonly HelperClass helperClass;

        public ConfirmAppoinmentHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IMediator mediator, HelperClass helperClass)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.mediator = mediator;
            this.helperClass = helperClass;
        }

        public async Task<Tuple<long, string>> Handle(ConfirmAppoinmentQuery request, CancellationToken cancellationToken)
        {
            int check = 0;

            // Fetch the appointment with status 1
            var appointment = await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetFirstAsync(y => y.Id == request.Id && y.AppointmentStatusId == 1, null, null, null);

            if (appointment != null)
            {
                // Begin transaction
                await using var transaction = await unitOfWork.BeginTransactionAsync();

                // Update appointment status
                var updateAppointment = unitOfWork.Repository<Entities.Models.Appointment>().GetFirst(y => y.Id == appointment.Id);
                updateAppointment.AppointmentStatusId = 5;
                updateAppointment.ModifiedById = sessionProvider.Session.LoggedInUserId;
                updateAppointment.ModifiedDate = DateTime.Now;
                updateAppointment.TokenNumber = await GenerateAppointmentCodeAsync(updateAppointment.DoctorId.Value, updateAppointment.AppointmentDate);
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
                var payment = await unitOfWork.Repository<AppointmentPayment>()
                    .GetFirstAsNoTrackingAsync(x => x.AppointmentId == appointment.Id, null, null, "Service");

                var serviceAccounts = await unitOfWork.Repository<Entities.Models.ServiceAccount>()
                .GetAsync(x => x.PaymentModeId == payment.PaymentModeId
                && x.ServiceTypeId == payment.Service.ServiceTypeId
                && x.ProjectId == sessionProvider.Session.SelectedWarehouseId, null, null, "PaymentMode", null, null);

                if (payment != null)
                {
                    payment.Discount = request.Discount; // Set status to 3
                    payment.TotalPayable = payment.VisitFee - request.Discount; // Set status to 3
                    payment.PaymentStatusId = 3; // Set status to 3
                    payment.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    payment.ModifiedDate = DateTime.Now;
                    payment.ApprovedById = sessionProvider.Session.LoggedInUserId;
                    payment.ApprovedDate = DateTime.Now;
                    unitOfWork.Repository<AppointmentPayment>().Update(payment);

                    var transactionCommand = helperClass.GetAppointmentVoucherCommandAsync(
                        updateAppointment,
                        payment,
                        serviceAccounts.ToList(),
                        request.Discount);

                    await mediator.Send(transactionCommand, cancellationToken);
                }

                await unitOfWork.SaveChangesAsync();

                // 6. Commit transaction
                await transaction.CommitAsync();
                return new Tuple<long, string>(200, "Appointment Confirmed Successfully!");
            }

            return new Tuple<long, string>(500, "Error Confirming, Please contact system admin!");
        }

        private async Task<string> GenerateMrnAsync()
        {
            var lastPatient = await unitOfWork.Repository<Entities.Models.Patient>()
                .GetOneAsync(
                    x => !string.IsNullOrEmpty(x.MRN) && x.ProjectId == sessionProvider.Session.SelectedWarehouseId,
                    q => q.OrderByDescending(x => x.Id));

            int next = 1;

            if (lastPatient != null &&
                int.TryParse(lastPatient.MRN, out int lastNo))
            {
                next = lastNo + 1;
            }

            return next.ToString("D6");
        }

        private async Task<string> GenerateAppointmentCodeAsync(Guid doctorId, DateTime appointmentDate)
        {
            var projectId = sessionProvider.Session.SelectedWarehouseId;

            var repository = unitOfWork.Repository<Entities.Models.Appointment>();

            var appointmentsForDay = await repository.FindAllAsync(
                x => x.IsActive
                     && x.DoctorId == doctorId
                     && x.ProjectId == projectId
                     && x.AppointmentDate.Date == appointmentDate.Date
                     && !string.IsNullOrEmpty(x.TokenNumber)
            );

            int nextNumber = 1;

            var maxNumber = appointmentsForDay
                .Select(x =>
                {
                    int.TryParse(x.TokenNumber, out int num);
                    return num;
                })
                .DefaultIfEmpty(0)
                .Max();

            nextNumber = maxNumber + 1;

            return nextNumber.ToString("D7");
        }
    }
}
