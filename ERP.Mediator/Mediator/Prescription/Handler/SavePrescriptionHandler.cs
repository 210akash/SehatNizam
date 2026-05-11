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
            Entities.Models.Prescription prescription;

            var existingPrescription = await unitOfWork.Repository<Entities.Models.Prescription>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            if (existingPrescription == null)
            {
                prescription = new Entities.Models.Prescription
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

                unitOfWork.Repository<Entities.Models.Prescription>()
                    .Add(prescription);
            }
            else
            {
                prescription = await unitOfWork.Repository<Entities.Models.Prescription>()
                    .GetFirstAsync(x => x.Id == request.Id);

                if (prescription == null)
                {
                    return -404;
                }

                prescription.AppointmentId = request.AppointmentId;
                prescription.DrugName = request.DrugName;
                prescription.Dosage = request.Dosage;
                prescription.Frequency = request.Frequency;
                prescription.Duration = request.Duration;
                prescription.Instructions = request.Instructions;
                prescription.ModifiedById = sessionProvider.Session.LoggedInUserId;
                prescription.ModifiedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.Prescription>()
                    .Update(prescription);
            }

            var check = await unitOfWork.SaveChangesAsync(cancellationToken);

            return check > 0 ? prescription.Id : 0;
        }
    }
}
