using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.UserAttendance.Query
{
    public class GetUserAttendanceByUserQuery : IRequest<List<GetUserAttendance>>
    {
        public Guid UserId { get; set; }
        public DateTime FDate { get; set; }
        public DateTime TDate { get; set; }
    }
}