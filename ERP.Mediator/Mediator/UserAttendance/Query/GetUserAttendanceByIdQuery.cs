using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.UserAttendance.Query
{
    public class GetUserAttendanceByIdQuery : IRequest<GetUserAttendance>
    {
        public GetUserAttendanceByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}