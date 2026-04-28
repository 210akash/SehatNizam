using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Appointment.Query
{
    public class GetAppointmentByTokenQuery : IRequest<GetAppointment>
    {
        public GetAppointmentByTokenQuery(string Token)
        {
            this.Token = Token;
        }

        public string Token { get; set; }
    }
}