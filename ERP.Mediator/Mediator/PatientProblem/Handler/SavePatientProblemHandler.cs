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

            var existingPatientProblem = await unitOfWork.Repository<Entities.Models.PatientProblem>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            if (existingPatientProblem == null)
            {
                var newPatientProblem = CreatePatientProblemFromCommand(request, isNew: true);
                unitOfWork.Repository<Entities.Models.PatientProblem>().Add(newPatientProblem);
            }
            else
            {
                var PatientProblemToUpdate = await unitOfWork.Repository<Entities.Models.PatientProblem>()
                    .GetFirstAsync(x => x.Id == request.Id);

                if (PatientProblemToUpdate != null)
                {
                    UpdatePatientProblemFromCommand(PatientProblemToUpdate, request);
                    unitOfWork.Repository<Entities.Models.PatientProblem>().Update(PatientProblemToUpdate);
                }

            }
            int check = await unitOfWork.SaveChangesAsync(cancellationToken);
            return 200;
        }

        private Entities.Models.PatientProblem CreatePatientProblemFromCommand(SavePatientProblemCommand command, bool isNew)
        {
            return new Entities.Models.PatientProblem
            {
                Id = command.Id,
                AppointmentId = command.AppointmentId,
                Problem = command.Problem,
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
            PatientProblem.StatusId = command.StatusId;
            PatientProblem.ModifiedById = sessionProvider.Session.LoggedInUserId;
            PatientProblem.ModifiedDate = DateTime.Now;
        }
    }

}