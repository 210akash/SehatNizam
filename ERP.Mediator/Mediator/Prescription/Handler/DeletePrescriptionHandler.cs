using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.Prescription.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Prescription.Handler
{
    public class DeletePrescriptionHandler : IRequestHandler<DeletePrescriptionCommand, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeletePrescriptionHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeletePrescriptionCommand request, CancellationToken cancellationToken)
        {
            var prescription = await unitOfWork.Repository<Entities.Models.Prescription>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            prescription.IsDelete = true;
            prescription.IsActive = false;
            prescription.DeleteDate = DateTime.Now;
            prescription.ModifiedDate = DateTime.Now;
            prescription.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.Prescription>().Update(prescription);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
