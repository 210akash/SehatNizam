using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Role.Query
{
    public class GetAllRoleQuery : IRequest<Tuple<IEnumerable<GetRoles>, long>>
    {
        public string? Id { get; set; }
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}