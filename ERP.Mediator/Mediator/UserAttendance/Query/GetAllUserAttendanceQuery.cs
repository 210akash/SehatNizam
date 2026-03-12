using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.UserAttendance.Query
{
    public class GetAllUserAttendanceQuery : IRequest<Tuple<IEnumerable<GetUserAttendance>, long>>
    {
        public string Name { get; set; }
        public string RoleId { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public PagingData PagingData { get; set; }
    }
}