using ERP.BusinessModels.ResponseVM;
using MediatR;

namespace ERP.Mediator.Mediator.Appointment.Query
{
    public class GetAppoinmentByIdQuery : IRequest<GetAppointment>
    {
        public GetAppoinmentByIdQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}