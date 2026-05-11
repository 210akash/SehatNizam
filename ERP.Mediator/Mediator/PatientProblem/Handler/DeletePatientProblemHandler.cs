using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.City.Query;
using ERP.Mediator.Mediator.PatientProblem.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.City.Handler
{
    public class DeletePatientProblemHandler : IRequestHandler<DeletePatientProblemCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeletePatientProblemHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeletePatientProblemCommand request, CancellationToken cancellationToken)
        {
            var patientProblem = await unitOfWork.Repository<Entities.Models.PatientProblem>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            patientProblem.IsDelete = true;
            patientProblem.IsActive = false;
            patientProblem.DeleteDate = DateTime.Now;
            patientProblem.ModifiedDate = DateTime.Now;
            patientProblem.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.PatientProblem>().Update(patientProblem);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
