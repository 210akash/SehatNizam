using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.EmployeeDocumentType.Query
{
    public class GetEmployeeDocumentTypeByNameQuery : IRequest<List<GetEmployeeDocumentType>>
    {
        public GetEmployeeDocumentTypeByNameQuery(string name)
        {
            this.name = name;
        }

        public string name { get; set; }
    }
}