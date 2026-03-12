using System;
using MediatR;

namespace ERP.Mediator.Mediator.SaleMaterial.Query
{
    public class GetSaleMaterialCountQuery : IRequest<Tuple<long, long, long, long>>
    {
        public string Code { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}