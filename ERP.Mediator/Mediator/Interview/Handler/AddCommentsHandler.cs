using System;
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

namespace ERP.Mediator.Mediator.Interview.Handler
{
    public class AddCommentsHandler : IRequestHandler<AddCommentsCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;
        private readonly IBlobService blobService;

        public AddCommentsHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider, IBlobService blobService)
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

        public async Task<long> Handle(AddCommentsCommand request, CancellationToken cancellationToken)
        {
            var existingInterview = await unitOfWork.Repository<Entities.Models.Interview>()
                .GetFirstAsNoTrackingAsync(x => x.Id == request.InterviewId);

            // Start Transaction
            using var transaction = await unitOfWork.Database().BeginTransactionAsync();
            try
            {
                existingInterview.StatusId = request.StatusId;
                existingInterview.CreatedById = existingInterview.CreatedById;
                existingInterview.CreatedDate = existingInterview.CreatedDate;
                existingInterview.ModifiedById = sessionProvider.Session.LoggedInUserId;
                existingInterview.ModifiedDate = DateTime.Now;

                unitOfWork.Repository<Entities.Models.Interview>().Update(existingInterview);
                SaveChanges();

                // Save interview history
                var interviewHistory = new InterviewHistory
                {
                    CreatedDate = DateTime.Now,
                    CreatedById = sessionProvider.Session.LoggedInUserId,
                    InterviewId = request.InterviewId,
                    InterviewDate = request.InterviewDate,
                    JoinAfterDays = request.JoinAfterDays,
                    Comments = request.Comments,
                    StatusId = request.StatusId,
                };

                await unitOfWork.Repository<InterviewHistory>().AddAsync(interviewHistory);
                SaveChanges(); // Ensure ID is generated

                if(request.InterviewAttendees != null)
                {
                    // Save interview attendees
                    foreach (var attendeeId in request.InterviewAttendees)
                    {
                        var interviewAttendee = new InterviewAttendees
                        {
                            CreatedDate = DateTime.Now,
                            CreatedById = sessionProvider.Session.LoggedInUserId,
                            InterviewHistoryId = interviewHistory.Id,
                            AspNetUsersId = new Guid(attendeeId)
                        };

                        await unitOfWork.Repository<InterviewAttendees>().AddAsync(interviewAttendee);
                    }

                    SaveChanges(); // Save all new related data
                }

                // Save Candidate Evaluations
                if ((request.StatusId == 180 || request.StatusId == 4)
                    && request.CandidateEvaluations != null
                    && request.CandidateEvaluations.Any())
                {
                    foreach (var item in request.CandidateEvaluations)
                    {
                        var evaluation = new CandidateEvaluation
                        {
                            CreatedDate = DateTime.Now,
                            CreatedById = sessionProvider.Session.LoggedInUserId,

                            InterviewHistoryId = interviewHistory.Id, // ✅ IMPORTANT FK
                            CandidateEvaluationCategoryId = item.CandidateEvaluationCategoryId,
                            CandidateScoringScaleId = item.CandidateScoringScaleId
                        };

                        await unitOfWork.Repository<CandidateEvaluation>().AddAsync(evaluation);
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