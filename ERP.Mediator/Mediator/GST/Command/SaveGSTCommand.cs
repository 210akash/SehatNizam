using MediatR;
using System;

namespace ERP.Mediator.Mediator.GST.Command
{
    public class SaveGSTCommand : IRequest<long>
    {
        public long Id { get; set; }
        public DateTime FDate { get; set; }
        public DateTime TDate { get; set; }
        public decimal GSTPer { get; set; }
        public long CompanyId { get; set; }
    }
}
