using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.CandidateEvaluationCategory.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.CandidateEvaluationCategory.Handler
{
    public class DeleteCandidateEvaluationCategoryHandler : IRequestHandler<DeleteCandidateEvaluationCategoryQuery, bool>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly SessionProvider sessionProvider;

        public DeleteCandidateEvaluationCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper, SessionProvider sessionProvider)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.sessionProvider = sessionProvider;
        }

        public async Task<bool> Handle(DeleteCandidateEvaluationCategoryQuery request, CancellationToken cancellationToken)
        {
            var CandidateEvaluationCategory = await unitOfWork.Repository<Entities.Models.CandidateEvaluationCategory>().GetFirstAsNoTrackingAsync(y => y.Id == request.Id);
            CandidateEvaluationCategory.IsDelete = true;
            CandidateEvaluationCategory.IsActive = false;
            CandidateEvaluationCategory.DeleteDate = DateTime.Now;
            CandidateEvaluationCategory.ModifiedDate = DateTime.Now;
            CandidateEvaluationCategory.ModifiedById = sessionProvider.Session.LoggedInUserId;
            unitOfWork.Repository<Entities.Models.CandidateEvaluationCategory>().Update(CandidateEvaluationCategory);
            await unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
