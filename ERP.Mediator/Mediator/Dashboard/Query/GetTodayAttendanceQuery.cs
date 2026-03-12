using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Dashboard.Query
{
    public class GetTodayAttendanceQuery : IRequest<GetTodayAttendance>
    {
        public GetTodayAttendanceQuery()
        {
        }
    }
}