using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.GRN.Command
{
    public class SavePurchaseInvoiceWHTCommand : IRequest<long>
    {
        public long Id { get; set; }
        public decimal? WHTPercentage { get; set; }
    }
}
