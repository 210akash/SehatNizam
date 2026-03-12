using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Interview.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Interview.Handler
{
    public class GetAllInterviewHandler : IRequestHandler<GetAllInterviewQuery, Tuple<IEnumerable<GetInterview>, long>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetAllInterviewHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<GetInterview>, long>> Handle(GetAllInterviewQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Entities.Models.Interview, bool>> predicate = x => x.IsActive == true
            ;

            Expression<Func<Entities.Models.Interview, object>>[] includes = {
                x => x.Department,
                x => x.Company,
                x => x.EmployeeDesignation,
                x => x.EmployeeEducation,
                x => x.CreatedBy,
                x => x.Status,
                x => x.InterviewHistory.Where(x => x.IsActive),
                x => x.Attachments.Where(x => x.IsActive),
            };

            List<string> thenIncludes = new List<string>();
            thenIncludes.Add("InterviewHistory.Status");
            thenIncludes.Add("InterviewHistory.CreatedBy");
            thenIncludes.Add("InterviewHistory.InterviewAttendees");
            thenIncludes.Add("InterviewHistory.InterviewAttendees.AspNetUsers");

            Expression<Func<Entities.Models.Interview, object>> OrderBy = null;
            Expression<Func<Entities.Models.Interview, object>> OrderByDesc = x => x.Id;
            var entity = unitOfWork.Repository<Entities.Models.Interview>().GetPagingWhereAsNoTrackingAsync(predicate, request.PagingData, OrderBy, OrderByDesc, thenIncludes, includes);

            var interview = mapper.Map<IEnumerable<GetInterview>>(entity.Item1.ToList()).ToList();

            return new Tuple<IEnumerable<GetInterview>, long>(interview, entity.Item2);
        }
    }
}
