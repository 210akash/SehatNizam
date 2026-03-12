using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ParameterVM;
using ERP.Core.Provider;
using ERP.Entities.Models;
using ERP.Mediator.Mediator.Interview.Command;
using ERP.Repositories.UnitOfWork;
using ERP.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ERP.Mediator.Mediator.Interview.Handler
{
    public class SaveInterviewHandler : IRequestHandler<SaveInterviewCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public SaveInterviewHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
            this.blobService = blobService;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        public async Task<long> Handle(SaveInterviewCommand request, CancellationToken cancellationToken)
        {
            var existingInterview = await unitOfWork.Repository<Entities.Models.Interview>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.Id);

            // Start Transaction
            using var transaction = await unitOfWork.Database().BeginTransactionAsync();
            try
            {
                Entities.Models.Interview interview;

                if (existingInterview == null)
                {
                    // Create new interview
                    interview = mapper.Map<Entities.Models.Interview>(request);
                    interview.StatusId = 1;
                    interview.CreatedById = sessionProvider.Session.LoggedInUserId;
                    interview.CreatedDate = DateTime.Now;

                    unitOfWork.Repository<Entities.Models.Interview>().Add(interview);
                    SaveChanges(); // Ensure ID is generated

                    // Save attachment if file is uploaded
                    if (request.FileCommand.Count() > 0)
                    {
                        foreach (var Doc in request.FileCommand)
                        {
                            //var attachment = new Attachments
                            //{
                            //    CreatedDate = DateTime.Now,
                            //    CreatedById = sessionProvider.Session.LoggedInUserId,
                            //    InterviewId = interview.Id,
                            //    ImageName = "/assets/Files/" + await blobService.UploadBase64FileToBlobAsync(new BlobImageUploadModel
                            //    {
                            //        File = item.FilePath,
                            //        FileName = item.FileName,
                            //        FolderName = "assets\\Files"
                            //    }, item.Extension)
                            //};
                            //await unitOfWork.Repository<Attachments>().AddAsync(attachment);
                            var _Doc = mapper.Map<Attachments>(Doc);
                            BlobImageUploadModel blobModel = new()
                            {
                                File = Doc.FilePath,
                                FileName = Doc.FileName,
                                FolderName = "assets/Files/HR"
                            };

                            _Doc.ImageName = "/assets/Files/HR/" + await blobService.UploadBase64FileToBlobAsync(blobModel, Doc.Extension);
                            _Doc.InterviewId = interview.Id;
                            _Doc.CreatedById = sessionProvider.Session.LoggedInUserId;
                            _Doc.CreatedDate = DateTime.Now;
                            unitOfWork.Repository<Attachments>().Add(_Doc);
                        }

                        SaveChanges();
                    }
                }
                else
                {
                    // Update existing interview
                    interview = mapper.Map<Entities.Models.Interview>(request);
                    interview.StatusId = existingInterview.StatusId;
                    interview.CreatedById = existingInterview.CreatedById;
                    interview.CreatedDate = existingInterview.CreatedDate;
                    interview.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    interview.ModifiedDate = DateTime.Now;

                    unitOfWork.Repository<Entities.Models.Interview>().Update(interview);
                    SaveChanges(); // Persist update

                    // Documents deletion

                    var docList = await unitOfWork.Repository<Attachments>()
                       .GetPagingWhereAsNoTrackingAsync(y => y.InterviewId == request.Id && y.IsActive == true,
                       null, null, null, null, null).Item1.ToListAsync();

                    List<long> previousdocIds = docList
                        .Select(y => y.Id)
                        .ToList();

                    List<long> currentdocIds = request.FileCommand.Select(y => y.Id).ToList();
                    List<long> deleteddocIds = previousdocIds.Except(currentdocIds).ToList();

                    // Handle deletions
                    foreach (var deleteddocId in deleteddocIds)
                    {
                        Attachments _Attachments = docList.Where(y => y.Id == deleteddocId).FirstOrDefault();

                        if (_Attachments != null)
                        {
                            _Attachments.ModifiedById = sessionProvider.Session.LoggedInUserId;
                            _Attachments.DeleteDate = DateTime.Now;
                            _Attachments.IsActive = false; // Soft delete
                            _Attachments.IsDelete = true; // Soft delete
                            unitOfWork.Repository<Attachments>().Update(_Attachments);
                        }
                    }

                    // Handle additions
                    foreach (var Doc in request.FileCommand)
                    {
                        if (Doc.Id != 0)
                        {

                        }
                        else
                        {
                            var _Doc = mapper.Map<Attachments>(Doc);
                            BlobImageUploadModel blobModel = new()
                            {
                                File = Doc.FilePath,
                                FileName = Doc.FileName,
                                FolderName = "assets/Files/HR"
                            };

                            _Doc.ImageName = "/assets/Files/HR/" + await blobService.UploadBase64FileToBlobAsync(blobModel, Doc.Extension);
                            _Doc.InterviewId = request.Id;
                            _Doc.CreatedById = sessionProvider.Session.LoggedInUserId;
                            _Doc.CreatedDate = DateTime.Now;
                            unitOfWork.Repository<Attachments>().Add(_Doc);
                        }
                    }

                    SaveChanges();
                }

                await transaction.CommitAsync(); // ✅ Commit transaction
                return 200;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(); // ❌ Rollback everything
                throw;
            }
        }


    }
}