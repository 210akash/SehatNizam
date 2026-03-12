using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.IGPType.Query
{
    public class GetAllIGPTypeQuery : IRequest<Tuple<IEnumerable<GetIGPType>, long>>
    {
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}