using ERP.BusinessModels.ParameterVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.RadiologyOrder.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.RadiologyOrder.Handler
{
    public class SaveRadiologyResultHandler : IRequestHandler<SaveRadiologyStudyResultCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public SaveRadiologyResultHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.blobService = blobService;
        }

        public async Task<long> Handle(SaveRadiologyStudyResultCommand request, CancellationToken cancellationToken)
        {
            var order = await unitOfWork.Repository<Entities.Models.RadiologyOrder>().GetFirstAsync(x => x.Id == request.RadiologyOrderId);

            if (order == null)
                return 0;

            RadiologyStudyResult result;

            if (request.Id > 0)
            {
                result = await unitOfWork
                    .Repository<RadiologyStudyResult>()
                    .GetFirstAsync(x => x.Id == request.Id);

                if (result == null)
                    return 0;

                result.PerformedById = request.PerformedById ?? sessionProvider.Session.LoggedInUserId;
                result.ReportedById = request.ReportedById ?? sessionProvider.Session.LoggedInUserId;
                result.PerformedDate = request.PerformedDate;
                result.ClinicalHistory = request.ClinicalHistory;
                result.Findings = request.Findings;
                result.Impression = request.Impression;
                result.Conclusion = request.Conclusion;

                result.ModifiedById = sessionProvider.Session.LoggedInUserId;
                result.ModifiedDate = DateTime.Now;

                unitOfWork.Repository<RadiologyStudyResult>().Update(result);
            }
            else
            {
                result = new RadiologyStudyResult
                {
                    RadiologyOrderId = request.RadiologyOrderId,
                    PerformedById = request.PerformedById ?? sessionProvider.Session.LoggedInUserId,
                    ReportedById = request.ReportedById ?? sessionProvider.Session.LoggedInUserId,
                    PerformedDate = request.PerformedDate,
                    ClinicalHistory = request.ClinicalHistory,
                    Findings = request.Findings,
                    Impression = request.Impression,
                    Conclusion = request.Conclusion,

                    CreatedById = sessionProvider.Session.LoggedInUserId,
                    CreatedDate = DateTime.Now
                };

                unitOfWork.Repository<RadiologyStudyResult>().Add(result);

                await unitOfWork.SaveChangesAsync();
            }

            if (request.Images != null)
            {
                var dbImages = await unitOfWork
                    .Repository<RadiologyStudyImage>()
                    .FindAllAsync(x => x.RadiologyStudyResultId == result.Id);

                var existingImages = dbImages.ToList();

                foreach (var dbImg in existingImages)
                {
                    var existsInRequest = request.Images.Any(x => x.Id == dbImg.Id);

                    if (!existsInRequest)
                    {
                        unitOfWork.Repository<RadiologyStudyImage>().Delete(dbImg);
                    }
                }

                foreach (var img in request.Images)
                {
                    var imageUrl = await ResolveImageUrlAsync(img);

                    if (img.Id > 0)
                    {
                        var existing = existingImages
                            .FirstOrDefault(x => x.Id == img.Id);

                        if (existing != null)
                        {
                            existing.ImageUrl = imageUrl;
                            existing.SequenceNo = img.SequenceNo;
                            existing.Remarks = img.Remarks;

                            existing.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            existing.ModifiedDate = DateTime.Now;

                            unitOfWork.Repository<RadiologyStudyImage>().Update(existing);
                        }
                    }
                    else
                    {
                        unitOfWork.Repository<RadiologyStudyImage>().Add(
                            new RadiologyStudyImage
                            {
                                RadiologyStudyResultId = result.Id,
                                ImageUrl = imageUrl,
                                SequenceNo = img.SequenceNo,
                                Remarks = img.Remarks,

                                CreatedById = sessionProvider.Session.LoggedInUserId,
                                CreatedDate = DateTime.Now
                            });
                    }
                }
            }

            order.StatusId = 15;
            order.ModifiedById = sessionProvider.Session.LoggedInUserId;
            order.ModifiedDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.RadiologyOrder>().Update(order);

            await unitOfWork.SaveChangesAsync();

            return result.Id;
        }

        private async Task<string> ResolveImageUrlAsync(SaveRadiologyStudyImageCommand img)
        {
            if (string.IsNullOrWhiteSpace(img.ImageUrl))
                return img.ImageUrl;

            if (!img.ImageUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return img.ImageUrl;

            var extension = string.IsNullOrWhiteSpace(img.Extension) ? "png" : img.Extension.TrimStart('.');
            var blobModel = new BlobImageUploadModel
            {
                File = img.ImageUrl,
                FileName = string.IsNullOrWhiteSpace(img.FileName) ? $"radiology-{Guid.NewGuid()}" : img.FileName,
                FolderName = "assets/Files/Radiology"
            };

            return "/assets/Files/Radiology/" + await blobService.UploadBase64FileToBlobAsync(blobModel, extension);
        }
    }
}
