using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.UserAttendance.Query
{
    public class GetZonesByUserInAttendanceQuery : IRequest<List<GetZone>>
    {
        public GetZonesByUserInAttendanceQuery(Guid UserId)
        {
            this.UserId = UserId;
        }
        public Guid? UserId { get; set; }
    }
}