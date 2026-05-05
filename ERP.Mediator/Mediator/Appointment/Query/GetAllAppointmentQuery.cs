using System;
using System.Collections.Generic;
using ERP.BusinessModels.ResponseVM;
using ERP.Entities.Models;
using MediatR;

namespace ERP.Mediator.Mediator.Appointment.Query
{
    public class GetAllAppointmentQuery : IRequest<Tuple<IEnumerable<GetAppointment>, long>>
    {
        public DateTime? FDate { get; set; }
        public DateTime? TDate { get; set; }
        public long? StatusId { get; set; }
        public PagingData PagingData { get; set; }
    }
}