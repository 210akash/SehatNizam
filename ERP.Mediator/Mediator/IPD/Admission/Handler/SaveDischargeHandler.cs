using AutoMapper;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.IPD.Admission.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class SaveDischargeHandler : IRequestHandler<SaveDischargeCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveDischargeHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<long> Handle(SaveDischargeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 2️⃣ Check if admission exists
                var admission = await unitOfWork.Repository<Admission>()
                    .GetFirstAsync(x => x.Id == request.AdmissionId);
                if (admission != null)
                {
                    var DischargeCertificate = await unitOfWork.Repository<DischargeCertificate>()
                        .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
                    if(DischargeCertificate == null)
                    {
                        var _DischargeCertificate = mapper.Map<DischargeCertificate>(request);
                        _DischargeCertificate.DischargeDateTime = request.DischargeDateTime;
                        unitOfWork.Repository<DischargeCertificate>().Add(_DischargeCertificate);
                        unitOfWork.SaveChanges();
                        return 200;
                    }
                    else
                    {
                        var _DischargeCertificate = mapper.Map<DischargeCertificate>(request);
                        _DischargeCertificate.CreatedById = DischargeCertificate.CreatedById;
                        _DischargeCertificate.CreatedDate = DischargeCertificate.CreatedDate;
                        _DischargeCertificate.ModifiedById = sessionProvider.Session.LoggedInUserId;
                        _DischargeCertificate.ModifiedDate = DateTime.Now;
                        unitOfWork.Repository<DischargeCertificate>().Update(_DischargeCertificate);
                        SaveChanges();
                        return 200;
                    }
                  
                }
                else
                {
                    return 404;
                }
            }
            catch
            {
                return 500;
                throw;
            }
        }
    }
}