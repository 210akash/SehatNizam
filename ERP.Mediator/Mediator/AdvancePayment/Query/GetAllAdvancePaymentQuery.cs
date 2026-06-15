using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.AdvancePayments.Query
{
    public class GetAllAdvancePaymentsQuery : IRequest<Tuple<IEnumerable<GetAdvancePayment>, long>>
    {
        public DateTime FDate { get; set; }
        public DateTime TDate { get; set; }
        public string PatientName { get; set; }
        public string MRN { get; set; }
        public long? StatusId { get; set; }
        public string AppointmentNo { get; set; }
        public PagingData PagingData { get; set; }

    }
}