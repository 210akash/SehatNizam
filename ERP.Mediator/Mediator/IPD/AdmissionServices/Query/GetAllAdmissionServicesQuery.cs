using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.IPD.AdmissionServices.Query
{
    public class GetAllAdmissionServicesQuery : IRequest<Tuple<IEnumerable<GetAppointmentPayment>, long>>
    {
        public long AppointmentId { get; set; }
        public PagingData PagingData { get; set; }

    }
}