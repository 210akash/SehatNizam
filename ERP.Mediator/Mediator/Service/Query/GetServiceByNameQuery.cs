using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Service.Query
{
    public class GetServiceByNameQuery : IRequest<List<GetService>>
    {
        public GetServiceByNameQuery(string Name,long? DepartmentId)
        {
            this.Name = Name;
            this.DepartmentId = DepartmentId;
        }

        public string Name { get; set; }
        public long? DepartmentId { get; set; }
    }
}