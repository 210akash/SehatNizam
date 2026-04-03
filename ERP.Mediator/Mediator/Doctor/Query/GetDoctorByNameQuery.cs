using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Doctor.Query
{
    public class GetDoctorByNameQuery : IRequest<List<GetEmployee>>
    {
        public GetDoctorByNameQuery(string Name, long DepartmentId)
        {
            this.Name = Name;
            this.DepartmentId = DepartmentId;
        }

        public string Name { get; set; }
        public long DepartmentId { get; set; }
    }
}