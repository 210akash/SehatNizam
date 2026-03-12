using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeLeave.Query
{
    public class GetAllEmployeeLeaveQuery : IRequest<Tuple<IEnumerable<GetEmployeeLeave>, long>>
    {
        public Guid? EmployeeId { get; set; }
        public long? DepartmentId { get; set; }
        public long StatusId { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public PagingData PagingData { get; set; }
    }
}