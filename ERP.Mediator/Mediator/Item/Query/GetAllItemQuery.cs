using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Item.Query
{
    public class GetAllItemQuery : IRequest<Tuple<IEnumerable<GetItem>, long>>
    {
        public long? ItemTypeId { get; set; }
        public long? CompanyId { get; set; }
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}