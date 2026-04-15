using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.Core.Provider;
using ERP.Mediator.Mediator.CandidateEvaluationCategory.Command;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.CandidateEvaluationCategory.Handler
{
    public class SaveCandidateEvaluationCategoryHandler : IRequestHandler<SaveCandidateEvaluationCategoryCommand, long>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly SessionProvider sessionProvider;

        public SaveCandidateEvaluationCategoryHandler(IMapper mapper, IUnitOfWork unitOfWork, SessionProvider sessionProvider)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.sessionProvider = sessionProvider;
        }

        public long SaveChanges()
        {
            return unitOfWork.SaveChanges();
        }

        async Task<long> IRequestHandler<SaveCandidateEvaluationCategoryCommand, long>.Handle(SaveCandidateEvaluationCategoryCommand request, CancellationToken cancellationToken)
        {
            var CandidateEvaluationCategory = await unitOfWork.Repository<Entities.Models.CandidateEvaluationCategory>().GetFirstAsNoTrackingAsync(x => x.Id == request.Id);
            var checkDuplicate = await unitOfWork.Repository<Entities.Models.CandidateEvaluationCategory>().GetAsync(x => x.Name.ToLower().Trim() == request.Name.ToLower().Trim() && x.IsActive == true && x.IsDelete == false && x.Id != request.Id);

            if (checkDuplicate.Count() == 0)
            {
                if (CandidateEvaluationCategory == null)
                {
                    var _CandidateEvaluationCategory = mapper.Map<Entities.Models.CandidateEvaluationCategory>(request);
                    _CandidateEvaluationCategory.CreatedById = sessionProvider.Session.LoggedInUserId;
                    _CandidateEvaluationCategory.CreatedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.CandidateEvaluationCategory>().Add(_CandidateEvaluationCategory);
                    SaveChanges();
                }
                else
                {
                    var _CandidateEvaluationCategory = mapper.Map<Entities.Models.CandidateEvaluationCategory>(request);
                    _CandidateEvaluationCategory.CreatedById = CandidateEvaluationCategory.CreatedById;
                    _CandidateEvaluationCategory.CreatedDate = CandidateEvaluationCategory.CreatedDate;
                    _CandidateEvaluationCategory.ModifiedById = sessionProvider.Session.LoggedInUserId;
                    _CandidateEvaluationCategory.ModifiedDate = DateTime.Now;
                    unitOfWork.Repository<Entities.Models.CandidateEvaluationCategory>().Update(_CandidateEvaluationCategory);
                    SaveChanges();
                }
                return 200;

            }
            else
            {
                return 409;
            }

        }
    }
}