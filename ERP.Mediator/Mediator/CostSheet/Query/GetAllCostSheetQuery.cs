using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.CostSheet.Query
{
    public class GetAllCostSheetQuery : IRequest<Tuple<IEnumerable<GetCostSheet>, long>>
    {
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public long? CompanyId { get; set; }
        public long? ItemId { get; set; }
        public string Code { get; set; }
        public long StatusId { get; set; }

        public PagingData PagingData { get; set; }
    }
}