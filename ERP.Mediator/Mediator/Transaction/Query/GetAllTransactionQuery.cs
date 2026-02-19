using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Transaction.Query
{
    public class GetAllTransactionQuery : IRequest<Tuple<IEnumerable<GetTransaction>, long>>
    {
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public long? CompanyId { get; set; }
        public string Code { get; set; }
        public long StatusId { get; set; }
        public long VoucherTypeId { get; set; }

        public PagingData PagingData { get; set; }
    }
}