using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Dashboard.Query
{
    public class GetTodayInterviewsQuery : IRequest<List<GetInterview>>
    {
        public GetTodayInterviewsQuery()
        {
        }
    }
}