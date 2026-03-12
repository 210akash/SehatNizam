using System;
using MediatR;

namespace ERP.Mediator.Mediator.GRN.Query
{
    public class GetPurchaseInvoiceCountQuery : IRequest<Tuple<long, long, long, long>>
    {
        public string Code { get; set; }
        public long? VendorId { get; set; }
        public string GRNCode { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}