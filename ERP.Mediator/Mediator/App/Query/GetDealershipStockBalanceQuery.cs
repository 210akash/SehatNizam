using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.App.Query
{
    public class GetDealershipStockBalanceQuery : IRequest<IEnumerable<GetDealershipStockBalance>>
    {
        public long DealershipId { get; set; }
        public DateTime AppDateTime { get; set; }
    }
}