using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.RadiologyOrder.Query
{
    public class GetAllRadiologyOrderQuery : IRequest<Tuple<IEnumerable<GetRadiologyOrder>, long>>
    {
        public long? RadiologyTypeId { get; set; }
        public long? StatusId { get; set; }
        public string TokenNo { get; set; }
        public string Name { get; set; }
        public string MRN { get; set; }
        public DateTime FDate { get; set; }
        public DateTime TDate { get; set; }
        public PagingData PagingData { get; set; }
    }
}
