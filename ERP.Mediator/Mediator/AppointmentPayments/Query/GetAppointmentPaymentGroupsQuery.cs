using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.AppointmentPayments.Query
{
    public class GetAppointmentPaymentGroupsQuery : IRequest<Tuple<IEnumerable<GetAppointmentPaymentGroup>, long>>
    {
        public DateTime FDate { get; set; }
        public DateTime TDate { get; set; }
        public string TokenNo { get; set; }
        public string MRN { get; set; }
        public string PatientName { get; set; }
        public long? PaymentStatusId { get; set; }
        public long? PaymentModeId { get; set; }
        public long? ServiceId { get; set; }
        public List<long> ServiceIds { get; set; }
        public PagingData PagingData { get; set; }
    }
}
