using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.LabOrder.Command;
using ERP.Mediator.Mediator.RadiologyOrder.Command;
using ERP.Repositories.UnitOfWork;
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
        public SaveRadiologyResultHandler(IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public async Task<long> Handle( SaveRadiologyStudyResultCommand request, CancellationToken cancellationToken)
        {
            var order = await unitOfWork.Repository<Entities.Models.RadiologyOrder>().GetFirstAsync(x => x.Id == request.RadiologyOrderId);

            if (order == null)
                return 0;

            RadiologyStudyResult result;

            // =========================
            // INSERT OR UPDATE RESULT
            // =========================
            if (request.Id > 0)
            {
                result = await unitOfWork
                    .Repository<RadiologyStudyResult>()
                    .GetFirstAsync(x => x.Id == request.Id);

                if (result == null)
                    return 0;

                result.PerformedById = request.PerformedById;
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
                    PerformedById = request.PerformedById,
                    ReportedById = request.ReportedById,
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

            // =========================
            // SAVE / SYNC IMAGES
            // =========================
            if (request.Images != null)
            {
                var dbImages = await unitOfWork
                    .Repository<RadiologyStudyImage>()
                    .FindAllAsync(x => x.RadiologyStudyResultId == result.Id);

                var existingImages = dbImages.ToList();

                // DELETE removed images
                foreach (var dbImg in existingImages)
                {
                    var existsInRequest = request.Images.Any(x => x.Id == dbImg.Id);

                    if (!existsInRequest)
                    {
                        unitOfWork.Repository<RadiologyStudyImage>().Delete(dbImg);
                    }
                }

                // ADD / UPDATE images
                foreach (var img in request.Images)
                {
                    if (img.Id > 0)
                    {
                        var existing = existingImages
                            .FirstOrDefault(x => x.Id == img.Id);

                        if (existing != null)
                        {
                            existing.ImageUrl = img.ImageUrl;
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
                                ImageUrl = img.ImageUrl,
                                SequenceNo = img.SequenceNo,
                                Remarks = img.Remarks,

                                CreatedById = sessionProvider.Session.LoggedInUserId,
                                CreatedDate = DateTime.Now
                            });
                    }
                }
            }

            // =========================
            // UPDATE ORDER STATUS
            // =========================
            order.StatusId = 15; // Reported (or your enum)
            order.ModifiedById = sessionProvider.Session.LoggedInUserId;
            order.ModifiedDate = DateTime.Now;

            unitOfWork.Repository<Entities.Models.RadiologyOrder>().Update(order);

            // =========================
            // SAVE ALL CHANGES
            // =========================
            await unitOfWork.SaveChangesAsync();

            return result.Id;
        }
    }
}
