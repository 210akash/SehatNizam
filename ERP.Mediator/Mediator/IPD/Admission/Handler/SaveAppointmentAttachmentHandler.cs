using ERP.BusinessModels.ParameterVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Appointment.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.Appointment.Handler
{
    public class SaveAppointmentAttachmentHandler : IRequestHandler<SaveAppointmentAttachmentCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public SaveAppointmentAttachmentHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.blobService = blobService;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<long> Handle(SaveAppointmentAttachmentCommand request, CancellationToken cancellationToken)
        {
            // 2️⃣ Check if appointment exists
            var appointment = await unitOfWork.Repository<Entities.Models.Appointment>()
                .GetFirstAsync(x => x.Id == request.AppointmentId);
            if (appointment != null)
            {
                foreach (var item in request.Files)
                {
                    AppointmentAttachment attachment = new AppointmentAttachment();
                    attachment.CreatedDate = DateTime.Now;
                    attachment.CreatedById = new Guid("408C1D72-07FD-4E9A-A54C-D1AD4112F875");
                    attachment.AppointmentId = request.AppointmentId;

                    BlobImageUploadModel blobModel = new()
                    {
                        File = item.FileSource,
                        FileName = item.ImageName,
                        FolderName = "assets/Files"
                    };

                    attachment.Attachment = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(blobModel, item.Extension);
                    await unitOfWork.Repository<AppointmentAttachment>().AddAsync(attachment);
                }
                unitOfWork.SaveChanges();
                return 200;
            }
            else
            {
                return 404;
            }
        }

    }
}