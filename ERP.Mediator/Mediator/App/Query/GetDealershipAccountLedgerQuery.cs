using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.App.Query
{
    public class GetDealershipAccountLedgerQuery : IRequest<IEnumerable<GetDealershipAccountLedger>>
    {
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public long DealershipId { get; set; }
        public DateTime AppDateTime { get; set; }
       // public PagingData PagingData { get; set; }
    }
}