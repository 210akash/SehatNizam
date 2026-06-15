using ERP.BusinessModels.ParameterVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Appointment.Command;
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
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public SaveDischargeHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.blobService = blobService;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<long> Handle(SaveDischargeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                SaveAppointmentAttachmentCommand command = new SaveAppointmentAttachmentCommand();

                // 2️⃣ Check if admission exists
                var admission = await unitOfWork.Repository<Admission>()
                    .GetFirstAsync(x => x.Id == request.AdmissionId);
                if (admission != null)
                {
                    foreach (var item in request.Files)
                    {
                        AppointmentAttachment attachment = new AppointmentAttachment();
                        attachment.CreatedDate = DateTime.Now;
                        attachment.CreatedById = sessionProvider.Session.LoggedInUserId;
                        attachment.AppointmentId = admission.AppointmentId;

                        BlobImageUploadModel blobModel = new()
                        {
                            File = item.FileSource,
                            FileName = item.ImageName,
                            FolderName = "assets/Files"
                        };

                        attachment.Attachment = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(blobModel, item.Extension);
                        await unitOfWork.Repository<AppointmentAttachment>().AddAsync(attachment);
                    }

                    admission.DischargeDate =  request.DischargeDate;
                    admission.DischargeSummary =  request.DischargeSummary;
                    admission.StatusId = 32;
                    unitOfWork.Repository<Admission>().Update(admission);
                    unitOfWork.SaveChanges();
                    return 200;
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