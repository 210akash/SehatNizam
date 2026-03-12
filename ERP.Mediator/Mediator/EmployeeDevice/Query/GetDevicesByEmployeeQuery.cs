using ERP.BusinessModels.ResponseVM;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.City.Query
{
    public class GetDevicesByEmployeeQuery : IRequest<List<GetEmployeeDevice>>
    {
        public GetDevicesByEmployeeQuery(Guid EmployeeId)
        {
            this.EmployeeId = EmployeeId;
        }

        public Guid EmployeeId { get; set; }
    }
}