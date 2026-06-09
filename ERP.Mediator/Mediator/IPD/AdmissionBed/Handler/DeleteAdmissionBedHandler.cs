using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.AdmissionBed.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionBed.Handler
{
    public class DeleteAdmissionBedHandler : IRequestHandler<DeleteAdmissionBedQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteAdmissionBedHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteAdmissionBedQuery request, CancellationToken cancellationToken)
        {
            var AdmissionBed = await unitOfWork.Repository<Entities.Models.AdmissionBed>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            AdmissionBed.IsDelete = true;
            AdmissionBed.IsActive = false;
            AdmissionBed.ModifiedDate = DateTime.Now;
            AdmissionBed.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.AdmissionBed>().Update(AdmissionBed);

            //Change bed Status
            var oldBedStatus = await unitOfWork.Repository<Entities.Models.Bed>().GetFirstAsNoTrackingAsync(x => x.Id == AdmissionBed.BedId && x.IsActive == true);
            oldBedStatus.IsOccupied = false;
            unitOfWork.Repository<Entities.Models.Bed>().Update(oldBedStatus);

            //Change Admission Status
            var AdmissionStatus = await unitOfWork.Repository<Entities.Models.Admission>().GetFirstAsNoTrackingAsync(x => x.Id == AdmissionBed.AdmissionId && x.IsActive == true);
            AdmissionStatus.StatusId = 30;
            unitOfWork.Repository<Entities.Models.Admission>().Update(AdmissionStatus);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
