using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.LabOrder.Query
{
    public class GetAllLabOrderQuery : IRequest<Tuple<IEnumerable<GetLabOrder>, long>>
    {
        public long? LabOrderTypeId { get; set; }
        public long? StatusId { get; set; }
        public string TokenNo { get; set; }
        public string Name { get; set; }
        public string MRN { get; set; }
        public DateTime FDate { get; set; }
        public DateTime TDate { get; set; }
        public PagingData PagingData { get; set; }
    }
}
