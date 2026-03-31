using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Auth.Query
{
    public class GetAllDoctorsQuery : IRequest<Tuple<List<GetAllUsers>, long>>
    {
        public string Name { get; set; }
        public long? DepartmentId { get; set; }
        public long? EmployeeDesignationId { get; set; }
        public PagingData PagingData { get; set; }
    }
}