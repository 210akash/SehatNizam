using MediatR;
using System;

namespace ERP.Mediator.Mediator.Appointment.Query
{
    public class ConfirmAppoinmentQuery : IRequest<Tuple<long, string>>
    {
        public ConfirmAppoinmentQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}