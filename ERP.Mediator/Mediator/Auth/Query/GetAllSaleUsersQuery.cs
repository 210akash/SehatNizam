using MediatR;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Auth.Query
{
    public class GetAllSaleUsersQuery : IRequest<Tuple<List<GetAllUsers>, long>>
    {
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public long? EmployeeDesignationId { get; set; }
        public long? EmployeeWorkSiteTypeId { get; set; }
        public PagingData PagingData { get; set; }
    }
}