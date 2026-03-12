using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Shop.Query
{
    public class GetShopsByTerritoryPagingQuery : IRequest<Tuple<IEnumerable<GetShopBasic>, long>>
    {
        public GetShopsByTerritoryPagingQuery(long TerritoryId,string Param, DateTime AppDateTime, PagingData PagingData)
        {
            this.TerritoryId = TerritoryId;
            this.Param = Param;
            this.AppDateTime = AppDateTime;
            this.PagingData = PagingData;
        }

        public long TerritoryId { get; set; }
        public string Param { get; set; }
        public DateTime AppDateTime { get; set; }
        public PagingData PagingData { get; set; }
    }
}