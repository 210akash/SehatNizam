using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Dashboard.Query
{
    public class GetHRDashboardQuery : IRequest<GetHRDashboardData>
    {
        public GetHRDashboardQuery()
        {
        }
    }
}