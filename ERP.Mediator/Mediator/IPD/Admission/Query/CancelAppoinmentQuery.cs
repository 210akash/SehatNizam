using MediatR;
using System;

namespace ERP.Mediator.Mediator.Appointment.Query
{
    public class CancelAppoinmentQuery : IRequest<Tuple<long, string>>
    {
        public CancelAppoinmentQuery(long Id)
        {
            this.Id = Id;
        }

        public long Id { get; set; }
    }
}