using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDocumentType.Query
{
    public class GetAllEmployeeDocumentTypeQuery : IRequest<Tuple<IEnumerable<GetEmployeeDocumentType>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}