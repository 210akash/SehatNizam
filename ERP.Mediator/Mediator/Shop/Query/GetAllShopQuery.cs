using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Shop.Query
{
    public class GetAllShopQuery : IRequest<Tuple<IEnumerable<GetShop>, long>>
    {
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public long RegionId { get; set; }
        public long ZoneId { get; set; }
        public long AreaId { get; set; }
        public long TerritoryId { get; set; }
        public long StatusId { get; set; }
        public string CreatedBy { get; set; }
        public string Name { get; set; }
        public PagingData PagingData { get; set; }
    }
}