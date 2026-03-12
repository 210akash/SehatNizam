using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ERP.BusinessModels.ResponseVM;
using ERP.Mediator.Mediator.Dashboard.Query;
using ERP.Repositories.UnitOfWork;
using MediatR;

namespace ERP.Mediator.Mediator.Dashboard.Handler
{
    public class GetTodayInterviewsHandler : IRequestHandler<GetTodayInterviewsQuery, List<GetInterview>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetTodayInterviewsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GetInterview>> Handle(GetTodayInterviewsQuery request, CancellationToken cancellationToken)
        {
            var interview = unitOfWork.Repository<Entities.Models.Interview>().GetAsync(x => x.IsActive && !x.IsDelete && x.StatusId == 2
            && x.InterviewHistory.Any(y => y.IsActive && !y.IsDelete && y.StatusId == 2 && y.InterviewDate.Value.Date == DateTime.Now.Date), null, null,
                    "Department," +
                    "Company," +
                    "EmployeeEducation," +
                    "EmployeeDesignation," +
                    "InterviewHistory," +
                    "InterviewHistory.CreatedBy," +
                    "InterviewHistory.Status," +
                    "InterviewHistory.InterviewAttendees," +
                    "InterviewHistory.InterviewAttendees.AspNetUsers"
                ).Result.ToList();

            var _map = mapper.Map<List<GetInterview>>(interview);
            return _map;
        }
    }
}
