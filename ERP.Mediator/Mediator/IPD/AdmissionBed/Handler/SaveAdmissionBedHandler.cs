using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.IPD.AdmissionBed.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionBed.Handler
{
    public class SaveAdmissionBedHandler : IRequestHandler<SaveAdmissionBedCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveAdmissionBedHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }
      
        async Task<long> IRequestHandler<SaveAdmissionBedCommand, long>.Handle(SaveAdmissionBedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var AdmissionStatus = await unitOfWork.Repository<Entities.Models.Admission>().GetFirstAsNoTrackingAsync(x => x.Id == request.AdmissionId && x.IsActive == true);
                var AdmissionBed = await unitOfWork.Repository<Entities.Models.AdmissionBed>().GetFirstAsNoTrackingAsync(x => x.AdmissionId == request.AdmissionId && x.IsActive == true);
                if (AdmissionBed != null)
                {
                    AdmissionBed.IsDelete = true;
                    AdmissionBed.IsActive = false;
                    AdmissionBed.ModifiedDate = DateTime.Now;
                    AdmissionBed.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    unitOfWork.Repository<Entities.Models.AdmissionBed>().Update(AdmissionBed);

                    //Change bed Status
                    var oldBedStatus = await unitOfWork.Repository<Entities.Models.Bed>().GetFirstAsNoTrackingAsync(x => x.Id == AdmissionBed.BedId && x.IsActive == true);
                    oldBedStatus.IsOccupied = false;
                    unitOfWork.Repository<Entities.Models.Bed>().Update(oldBedStatus);
                }

                var _AdmissionBed = mapper.Map<Entities.Models.AdmissionBed>(request);
                _AdmissionBed.CreatedById = sessionProvider.Session.LoggedInUserId;
                _AdmissionBed.CreatedDate = DateTime.Now;
                unitOfWork.Repository<Entities.Models.AdmissionBed>().Add(_AdmissionBed);

                //Change bed Status
                var newBedStatus = await unitOfWork.Repository<Entities.Models.Bed>().GetFirstAsNoTrackingAsync(x => x.Id == request.BedId && x.IsActive == true);
                newBedStatus.IsOccupied = true;
                unitOfWork.Repository<Entities.Models.Bed>().Update(newBedStatus);

                //Change Admission Status
                AdmissionStatus.StatusId = 31;
                unitOfWork.Repository<Entities.Models.Admission>().Update(AdmissionStatus);
                SaveChanges();
                return 200;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}