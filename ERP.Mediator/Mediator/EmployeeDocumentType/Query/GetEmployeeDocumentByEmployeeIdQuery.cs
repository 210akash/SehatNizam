using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDocumentType.Query
{
    public class GetEmployeeDocumentByEmployeeIdQuery : IRequest<List<GetEmployeeDocument>>
    {
        public GetEmployeeDocumentByEmployeeIdQuery(Guid EmployeeId)
        {
            this.EmployeeId = EmployeeId;
        }

        public Guid EmployeeId { get; set; }
    }
}