using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Appointment.Query
{
    public class GetAppointmentsByBookingNoQuery : IRequest<List<GetAppointment>>
    {
        public GetAppointmentsByBookingNoQuery(string BookingNo)
        {
            this.BookingNo = BookingNo;
        }

        public string BookingNo { get; set; }
    }
}