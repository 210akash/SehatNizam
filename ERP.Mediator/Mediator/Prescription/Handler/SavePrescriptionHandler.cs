using ERP.Core.Provider;
using ERP.Mediator.Mediator.Prescription.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Prescription.Handler
{
    public class SavePrescriptionHandler : IRequestHandler<SavePrescriptionCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SavePrescriptionHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle(SavePrescriptionCommand request, CancellationToken cancellationToken)
        {
            var existingPrescription = await unitOfWork.Repository<Entities.Models.Prescription>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            if (existingPrescription == null)
            {
                var prescription = new Entities.Models.Prescription
                {
                    AppointmentId = request.AppointmentId,
                    DrugName = request.DrugName,
                    Dosage = request.Dosage,
                    Frequency = request.Frequency,
                    Duration = request.Duration,
                    Instructions = request.Instructions,
                    CreatedById = sessionProvider.Session.LoggedInUserId,
                    CreatedDate = DateTime.Now
                };

                unitOfWork.Repository<Entities.Models.Prescription>().Add(prescription);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return 200;
            }

            var prescriptionToUpdate = await unitOfWork.Repository<Entities.Models.Prescription>()
                .GetFirstAsync(x => x.Id == request.Id);

            if (prescriptionToUpdate == null)
            {
                return 404;
            }

            prescriptionToUpdate.AppointmentId = request.AppointmentId;
            prescriptionToUpdate.DrugName = request.DrugName;
            prescriptionToUpdate.Dosage = request.Dosage;
            prescriptionToUpdate.Frequency = request.Frequency;
            prescriptionToUpdate.Duration = request.Duration;
            prescriptionToUpdate.Instructions = request.Instructions;
            prescriptionToUpdate.ModifiedById = sessionProvider.Session.LoggedInUserId;
            prescriptionToUpdate.ModifiedDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.Prescription>().Update(prescriptionToUpdate);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return 200;
        }
    }
}
