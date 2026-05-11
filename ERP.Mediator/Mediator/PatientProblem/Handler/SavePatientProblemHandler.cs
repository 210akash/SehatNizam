using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.PatientProblem.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.PatientProblem.Handler
{
    public class SavePatientProblemHandler : IRequestHandler<SavePatientProblemCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        public SavePatientProblemHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<long> Handle(SavePatientProblemCommand request, CancellationToken cancellationToken)
        {
            Entities.Models.PatientProblem patientProblem;

            var existingPatientProblem = await unitOfWork.Repository<Entities.Models.PatientProblem>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            if (existingPatientProblem == null)
            {
                patientProblem = CreatePatientProblemFromCommand(request, isNew: true);

                unitOfWork.Repository<Entities.Models.PatientProblem>()
                    .Add(patientProblem);
            }
            else
            {
                patientProblem = await unitOfWork.Repository<Entities.Models.PatientProblem>()
                    .GetFirstAsync(x => x.Id == request.Id);

                if (patientProblem == null)
                {
                    return -404;
                }

                UpdatePatientProblemFromCommand(patientProblem, request);

                unitOfWork.Repository<Entities.Models.PatientProblem>()
                    .Update(patientProblem);
            }

            int check = await unitOfWork.SaveChangesAsync(cancellationToken);

            return check > 0 ? patientProblem.Id : 0;
        }

        private Entities.Models.PatientProblem CreatePatientProblemFromCommand(SavePatientProblemCommand command, bool isNew)
        {
            return new Entities.Models.PatientProblem
            {
                Id = command.Id,
                AppointmentId = command.AppointmentId,
                Problem = command.Problem,
                Onset = command.Onset,
                StatusId = command.StatusId,
                CreatedById = sessionProvider.Session.LoggedInUserId,
                CreatedDate = DateTime.Now,
                ModifiedById = isNew ? null : sessionProvider.Session.LoggedInUserId,
                ModifiedDate = isNew ? null : DateTime.Now
            };
        }

        private void UpdatePatientProblemFromCommand(Entities.Models.PatientProblem PatientProblem, SavePatientProblemCommand command)
        {
            PatientProblem.AppointmentId = command.AppointmentId;
            PatientProblem.Problem = command.Problem;
            PatientProblem.Onset = command.Onset;
            PatientProblem.StatusId = command.StatusId;
            PatientProblem.ModifiedById = sessionProvider.Session.LoggedInUserId;
            PatientProblem.ModifiedDate = DateTime.Now;
        }
    }

}