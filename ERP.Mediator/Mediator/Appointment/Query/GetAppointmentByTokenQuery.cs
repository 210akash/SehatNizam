using ERP.BusinessModels.ResponseVM;
using MediatR;
using System.Collections.Generic;

namespace ERP.Mediator.Mediator.Appointment.Query
{
    public class GetAppointmentByTokenQuery : IRequest<List<GetAppointment>>
    {
        public GetAppointmentByTokenQuery(string Token,long StatusId)
        {
            this.Token = Token;
            this.StatusId = StatusId;
        }

        public string Token { get; set; }
        public long StatusId { get; set; }
    }
}