using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.ItemType.Query
{
    public class GetAllItemTypeQuery : IRequest<Tuple<IEnumerable<GetItemType>, long>>
    {
        public long? SubCategoryId { get; set; }
        public long? CompanyId { get; set; }
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}