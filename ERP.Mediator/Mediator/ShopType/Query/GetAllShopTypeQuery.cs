using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.ShopType.Query
{
    public class GetAllShopTypeQuery : IRequest<Tuple<IEnumerable<GetShopType>, long>>
    {
        public long? ShopTypeId { get; set; }
        public string Name { get; set; }

        public PagingData PagingData { get; set; }
    }
}