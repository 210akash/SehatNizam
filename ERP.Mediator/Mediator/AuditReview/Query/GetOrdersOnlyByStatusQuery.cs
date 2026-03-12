using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Mediator.Mediator.AuditReview.Query
{
    public class GetOrdersOnlyByStatusQuery : IRequest<Tuple<IEnumerable<GetOrder>, long>>
    {
        public long? StatusId { get; set; }
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public string Code { get; set; }
        public long DealershipId { get; set; }
        public PagingData PagingData { get; set; }
    }
}
