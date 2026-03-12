using MediatR;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using System;

namespace ERP.Mediator.Mediator.Auth.Query
{
    public class GetAllUsersQuery : IRequest<Tuple<List<GetAllUsers>, long>>
    {
        public string Name { get; set; }
        public string CNIC { get; set; }
        public string HrCode { get; set; }
        public long DepartmentId { get; set; }
        public long EmployeeWorkSiteTypeId { get; set; }
        public PagingData PagingData { get; set; }
    }
}