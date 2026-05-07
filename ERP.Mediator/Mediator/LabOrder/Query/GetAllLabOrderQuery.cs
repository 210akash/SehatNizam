using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.LabOrder.Query
{
    public class GetAllLabOrderQuery : IRequest<Tuple<IEnumerable<GetLabOrder>, long>>
    {
        public long? AppointmentId { get; set; }
        public PagingData PagingData { get; set; }
    }
}
