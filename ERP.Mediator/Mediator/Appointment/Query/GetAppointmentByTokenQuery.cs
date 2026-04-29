using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Appointment.Query
{
    public class GetAppointmentByTokenQuery : IRequest<List<GetAppointment>>
    {
        public GetAppointmentByTokenQuery(string Token)
        {
            this.Token = Token;
        }

        public string Token { get; set; }
    }
}