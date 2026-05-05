using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Triage.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Triage.Handler
{
    public class SaveTriageHandler : IRequestHandler<SaveTriageCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public SaveTriageHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<long> Handle(SaveTriageCommand request, CancellationToken cancellationToken)
        {

            // 2️⃣ Check if appointment exists
            var appointment = await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetFirstAsync(x => x.Id == request.AppointmentId);
            if (appointment != null)
            {

                var existingTriage = await unitOfWork.Repository<Entities.Models.Triage>()
                    .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

                if (existingTriage == null)
                {
                    var newTriage = CreateTriageFromCommand(request, isNew: true);
                    unitOfWork.Repository<Entities.Models.Triage>().Add(newTriage);
                }
                else
                {
                    var triageToUpdate = await unitOfWork.Repository<Entities.Models.Triage>()
                        .GetFirstAsync(x => x.Id == request.Id);

                    if (triageToUpdate != null)
                    {
                        UpdateTriageFromCommand(triageToUpdate, request);
                        unitOfWork.Repository<Entities.Models.Triage>().Update(triageToUpdate);
                    }

                }
                appointment.AppointmentStatusId = 10;
                unitOfWork.Repository<Entities.Models.Appointment>().Update(appointment);
                int check = await unitOfWork.SaveChangesAsync(cancellationToken);
                return 200;
            }
            else
            {
                return 404;
            }
        }

        private Entities.Models.Triage CreateTriageFromCommand(SaveTriageCommand command, bool isNew)
        {
            return new Entities.Models.Triage
            {
                Id = command.Id,
                AppointmentId = command.AppointmentId,
                NurseId = sessionProvider.Session.LoggedInUserId,
                Temperature = command.Temperature,
                Pulse = command.Pulse,
                SystolicBp = command.SystolicBp,
                DiastolicBp = command.DiastolicBp,
                Spo2 = command.Spo2,
                Weight = command.Weight,
                HeightFeet = command.HeightFeet,
                HeightInches = command.HeightInches,
                HeightCm = command.HeightCm,
                Bmi = command.Bmi,
                BloodSugar = command.BloodSugar,
                SugarTypeId = command.SugarTypeId,
                TriagePriorityId = command.TriagePriorityId,
                ChiefComplaint = command.ChiefComplaint,
                Allergies = command.Allergies,
                Medications = command.Medications,
                Notes = command.Notes,
                TriageScore = command.TriageScore,
                CreatedById =  sessionProvider.Session.LoggedInUserId,
                CreatedDate = DateTime.Now,
                ModifiedById = isNew ? null : sessionProvider.Session.LoggedInUserId,
                ModifiedDate = isNew ? null : DateTime.Now
            };
        }

        private void UpdateTriageFromCommand(Entities.Models.Triage triage, SaveTriageCommand command)
        {
            triage.AppointmentId = command.AppointmentId;
            triage.Temperature = command.Temperature;
            triage.Pulse = command.Pulse;
            triage.SystolicBp = command.SystolicBp;
            triage.DiastolicBp = command.DiastolicBp;
            triage.Spo2 = command.Spo2;
            triage.Weight = command.Weight;
            triage.HeightFeet = command.HeightFeet;
            triage.HeightInches = command.HeightInches;
            triage.HeightCm = command.HeightCm;
            triage.Bmi = command.Bmi;
            triage.BloodSugar = command.BloodSugar;
            triage.SugarTypeId = command.SugarTypeId;
            triage.TriagePriorityId = command.TriagePriorityId;
            triage.ChiefComplaint = command.ChiefComplaint;
            triage.Allergies = command.Allergies;
            triage.Medications = command.Medications;
            triage.Notes = command.Notes;
            triage.TriageScore = command.TriageScore;
            triage.ModifiedById = sessionProvider.Session.LoggedInUserId;
            triage.ModifiedDate = DateTime.Now;
        }
    }
}