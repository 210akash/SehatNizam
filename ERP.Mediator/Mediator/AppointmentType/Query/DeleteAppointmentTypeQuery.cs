using MediatR;

namespace ERP.Mediator.Mediator.AppointmentType.Query
{
    public class DeleteAppointmentTypeQuery : IRequest<bool>
    {
        public DeleteAppointmentTypeQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}